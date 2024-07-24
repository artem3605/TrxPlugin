﻿using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Linq;
using System.Threading;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.Rd.Tasks;
using JetBrains.ReSharper.Feature.Services.Protocol;
using Rider.Plugins.SimplePlugin.Model;
using JetBrains.ReSharper.UnitTestFramework.Execution;
using JetBrains.ReSharper.UnitTestFramework.Session;
using JetBrains.ReSharper.UnitTestFramework.UI.Session;

namespace Rider.Plugins.SimplePlugin;

[SolutionComponent]
class TrxManager
{
    private readonly IUnitTestSessionRepository myRepository;
    private readonly IUnitTestSessionConductor mySessionConductor;
    private readonly IUnitTestResultManager myResultManager;
    private readonly RdSimplePluginModel myModel;
    private string output = "";
    public TrxManager(Lifetime lifetime, IUnitTestSessionRepository repository, IUnitTestSessionConductor sessionConductor,
        IUnitTestResultManager resultManager, ISolution solution)
    {
        myRepository = repository;
        mySessionConductor = sessionConductor;
        myResultManager = resultManager;
        myModel = solution.GetProtocolSolution().GetRdSimplePluginModel();
        myModel.MyCall.SetAsync(HandleCall);
    }

    private async Task<bool> Do(string trxFilePath)
    {
        await Task.Delay(10000);
        XDocument document;
        await using (var stream = File.OpenRead(trxFilePath))
        {
            document = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        }
        var root = document.Root;
        if (root == null)
        {
            return false;
        }

        XNamespace ns = root.GetDefaultNamespace();
        var results = document.Descendants(ns + "UnitTestResult");

        foreach (var result in results)
        {
            string testName = result.Attribute("testName")?.Value;
            string outcome = result.Attribute("outcome")?.Value;
            ns = result.Name.Namespace;

            if (outcome == "Failed")
            {
                var messageElem = result.Descendants(ns + "Message").FirstOrDefault();
                var stackTraceElem = result.Descendants(ns + "StackTrace").FirstOrDefault();
            }

            if (outcome == "Warn")
            {
                var messageElem = result.Descendants(ns + "Message").FirstOrDefault();
            }

            var stdOutElem = result.Descendants(ns + "StdOut").FirstOrDefault();
            output += stdOutElem?.Value;
        }
        /*
        myResultManager.TestFinishing();
        myResultManager.TestException();
        myRepository.Query(new TestIdCriterion());

        var unitTestSession = myRepository.CreateSession(new TestElementCriterion());

        mySessionConductor.OpenSession(unitTestSession);
        */
        return true;
    }
    private async Task<RdCallResponse> HandleCall(Lifetime lt, RdCallRequest request)
    {
        string path = request.MyField;
        if (await Do(path))
        {
            return lt.Execute(() => new RdCallResponse("Success"));
        }
        return lt.Execute(() => new RdCallResponse("Failed"));
    }
}