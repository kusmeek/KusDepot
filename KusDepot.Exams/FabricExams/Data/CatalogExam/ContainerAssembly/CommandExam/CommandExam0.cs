[assembly:KusDepot.CommandContainer(typeof(CommandExam0),"Handle001","Specification Sheet")]
[assembly:KusDepot.CommandContainer(typeof(CommandExam0),"Handle002","Usage...")]

namespace KusDepot.Exams;

public class CommandExam0 : Command
{
    public override Guid? Execute(Activity? activity , CommandKey? key = null)
    {
        throw new NotImplementedException();
    }
}