using System.Xml.Linq;
using Rider.Plugins.TrxPlugin;

namespace Tests;

public class UnitTestParseResults
{
    private int _passed;
    private int _failed;
    private int _warning;

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        XDocument document;
        using (var stream = File.OpenRead("../../../TestData/test1.trx"))
        {
            document = XDocument.Load(stream);
        }

        var root = document.Root;

        var results = TrxParser.ParseResults(root, root?.GetDefaultNamespace());
        Assert.That(results.Count, Is.EqualTo(1));
        Assert.That(results[0].TestName, Is.EqualTo("ParentTest"));
        Assert.That(results[0].Outcome, Is.EqualTo("Passed"));
        Assert.That(TimeSpan.Parse(results[0].Duration), Is.EqualTo(TimeSpan.FromDays(1)));
        Assert.That(results[0].Output.ErrorInfo, Is.Null);
        Assert.That(results[0].InnerResults.UnitTestResults[0].TestName, Is.EqualTo("ChildTest1"));
        Assert.That(results[0].InnerResults.UnitTestResults[1].TestName, Is.EqualTo("ChildTest2"));
    }

    [Test]
    public void Test2()
    {
        XDocument document;
        using (var stream = File.OpenRead("../../../TestData/test2.trx"))
        {
            document = XDocument.Load(stream);
        }

        var root = document.Root;

        var results = TrxParser.ParseResults(root, root?.GetDefaultNamespace());
        Assert.That(results.Count, Is.EqualTo(20));
        foreach (var result in results)
        {
            if (result.Outcome.ToLower() == "passed")
            {
                _passed += 1;
            }

            if (result.Outcome.ToLower() == "failed")
            {
                _failed += 1;
            }

            if (result.Outcome.ToLower() == "warn")
            {
                _warning += 1;
            }
        }

        Assert.That(_passed, Is.EqualTo(6));
        Assert.That(_failed, Is.EqualTo(13));
        Assert.That(_warning, Is.EqualTo(1));
    }

    [Test]
    public void Test3()
    {
        XDocument document;
        using (var stream = File.OpenRead("../../../TestData/test3.trx"))
        {
            document = XDocument.Load(stream);
        }

        var root = document.Root;

        var results = TrxParser.ParseResults(root, root?.GetDefaultNamespace());
        Assert.That(results.Count, Is.EqualTo(2));
        Assert.That(results[1].TestName, Is.EqualTo("IndependentTest"));
        Assert.That(results[1].Outcome, Is.EqualTo("Passed"));
        Assert.That(results[0].TestName, Is.EqualTo("MainTest"));
        Assert.That(results[0].Outcome, Is.EqualTo("Failed"));
        Assert.That(results[0].InnerResults?.UnitTestResults?.Count, Is.EqualTo(2));
        Assert.That(results[0].InnerResults.UnitTestResults[0].TestName, Is.EqualTo("SubTest1"));
        Assert.That(results[0].InnerResults.UnitTestResults[1].TestName, Is.EqualTo("SubTest2"));
        Assert.That(results[0].InnerResults.UnitTestResults[0].Outcome, Is.EqualTo("Passed"));
        Assert.That(results[0].InnerResults.UnitTestResults[1].Outcome, Is.EqualTo("Failed"));
    }

    [Test]
    public void Test4()
    {
        XDocument document;
        using (var stream = File.OpenRead("../../../TestData/test4.trx"))
        {
            document = XDocument.Load(stream);
        }

        var root = document.Root;

        var results = TrxParser.ParseResults(root, root?.GetDefaultNamespace());
        Assert.That(results.Count, Is.EqualTo(1));
        Assert.That(results[0].TestName, Is.EqualTo("MainTest"));
        Assert.That(results[0].Outcome, Is.EqualTo("Passed"));
        Assert.That(results[0].InnerResults?.UnitTestResults?.Count, Is.EqualTo(2));
        Assert.That(results[0].InnerResults.UnitTestResults[0].TestName, Is.EqualTo("SubTest1"));
        Assert.That(results[0].InnerResults.UnitTestResults[1].TestName, Is.EqualTo("SubTest2"));
        Assert.That(results[0].InnerResults.UnitTestResults[0].Outcome, Is.EqualTo("Failed"));
        Assert.That(results[0].InnerResults.UnitTestResults[1].Outcome, Is.EqualTo("Failed"));
        Assert.That(results[0].InnerResults.UnitTestResults[0].InnerResults?.UnitTestResults?.Count, Is.EqualTo(2));
        Assert.That(results[0].InnerResults.UnitTestResults[0].InnerResults.UnitTestResults[0].TestName,
            Is.EqualTo("SubSubTest1"));
        Assert.That(results[0].InnerResults.UnitTestResults[0].InnerResults.UnitTestResults[1].TestName,
            Is.EqualTo("SubSubTest2"));
        Assert.That(results[0].InnerResults.UnitTestResults[0].InnerResults.UnitTestResults[0].Outcome,
            Is.EqualTo("Failed"));
        Assert.That(results[0].InnerResults.UnitTestResults[0].InnerResults.UnitTestResults[1].Outcome,
            Is.EqualTo("Failed"));
    }

    [Test]
    public void Test5()
    {
        XDocument document;
        using (var stream = File.OpenRead("../../../TestData/test5.trx"))
        {
            document = XDocument.Load(stream);
        }
        var root = document.Root;

        var results = TrxParser.ParseResults(root, root?.GetDefaultNamespace());
        Assert.That(results.Count, Is.EqualTo(1));
        Assert.That(results[0].TestName, Is.EqualTo("TestMethod1"));
        Assert.That(results[0].Outcome, Is.EqualTo("Failed"));
    }
}
