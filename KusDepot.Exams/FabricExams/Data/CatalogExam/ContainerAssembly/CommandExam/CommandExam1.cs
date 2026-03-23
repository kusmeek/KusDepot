[assembly:KusDepot.CommandContainer(typeof(CommandExam1),"Handle001","Specification Sheet")]
[assembly:KusDepot.CommandContainer(typeof(CommandExam1),"Handle002","Usage...")]

namespace KusDepot.Exams;

public class CommandExam1 : Command
{
    public override Guid? Execute(Activity? activity , CommandKey? key = null)
    {
        throw new NotImplementedException();
    }
}