namespace KusDepot.Exams;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ReflectionBindingScoreExam
{
    [Test]
    public void Compare_ShorterLength_Wins()
    {
        Int32 cmp = ReflectionBindingScore.Compare(
            length:1,exact:0,assignable:0,isstatic:false,
            bestLength:2,bestExact:0,bestAssignable:0,bestStatic:null,
            preferInstanceOnTie:false);

        Assert.That(cmp,Is.EqualTo(1));
    }

    [Test]
    public void Compare_LongerLength_Loses()
    {
        Int32 cmp = ReflectionBindingScore.Compare(
            length:3,exact:0,assignable:0,isstatic:false,
            bestLength:2,bestExact:0,bestAssignable:0,bestStatic:null,
            preferInstanceOnTie:false);

        Assert.That(cmp,Is.EqualTo(-1));
    }

    [Test]
    public void Compare_HigherExact_WinsWhenLengthEqual()
    {
        Int32 cmp = ReflectionBindingScore.Compare(
            length:2,exact:2,assignable:0,isstatic:false,
            bestLength:2,bestExact:1,bestAssignable:1,bestStatic:null,
            preferInstanceOnTie:false);

        Assert.That(cmp,Is.EqualTo(1));
    }

    [Test]
    public void Compare_LowerExact_LosesWhenLengthEqual()
    {
        Int32 cmp = ReflectionBindingScore.Compare(
            length:2,exact:0,assignable:5,isstatic:false,
            bestLength:2,bestExact:1,bestAssignable:0,bestStatic:null,
            preferInstanceOnTie:false);

        Assert.That(cmp,Is.EqualTo(-1));
    }

    [Test]
    public void Compare_HigherAssignable_WinsWhenLengthAndExactEqual()
    {
        Int32 cmp = ReflectionBindingScore.Compare(
            length:2,exact:1,assignable:2,isstatic:false,
            bestLength:2,bestExact:1,bestAssignable:1,bestStatic:null,
            preferInstanceOnTie:false);

        Assert.That(cmp,Is.EqualTo(1));
    }

    [Test]
    public void Compare_LowerAssignable_LosesWhenLengthAndExactEqual()
    {
        Int32 cmp = ReflectionBindingScore.Compare(
            length:2,exact:1,assignable:0,isstatic:false,
            bestLength:2,bestExact:1,bestAssignable:1,bestStatic:null,
            preferInstanceOnTie:false);

        Assert.That(cmp,Is.EqualTo(-1));
    }

    [Test]
    public void Compare_PreferInstanceOnTie_WinsAgainstStatic()
    {
        Int32 cmp = ReflectionBindingScore.Compare(
            length:2,exact:1,assignable:1,isstatic:false,
            bestLength:2,bestExact:1,bestAssignable:1,bestStatic:true,
            preferInstanceOnTie:true);

        Assert.That(cmp,Is.EqualTo(1));
    }

    [Test]
    public void Compare_PreferInstanceOnTie_StaticLosesAgainstInstance()
    {
        Int32 cmp = ReflectionBindingScore.Compare(
            length:2,exact:1,assignable:1,isstatic:true,
            bestLength:2,bestExact:1,bestAssignable:1,bestStatic:false,
            preferInstanceOnTie:true);

        Assert.That(cmp,Is.EqualTo(-1));
    }

    [Test]
    public void Compare_FullTie_ReturnsAmbiguousZero()
    {
        Int32 cmp = ReflectionBindingScore.Compare(
            length:2,exact:1,assignable:1,isstatic:false,
            bestLength:2,bestExact:1,bestAssignable:1,bestStatic:false,
            preferInstanceOnTie:true);

        Assert.That(cmp,Is.EqualTo(0));
    }
}
