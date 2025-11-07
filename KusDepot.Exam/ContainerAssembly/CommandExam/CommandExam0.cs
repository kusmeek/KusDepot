[assembly:KusDepot.CommandContainer(typeof(KusDepot.Exams.CommandExam0),"Handle001","Specification Sheet")]
[assembly:KusDepot.CommandContainer(typeof(KusDepot.Exams.CommandExam0),"Handle002","Usage...")]

namespace KusDepot.Exams;

public class CommandExam0 : Command
{
    public override Guid? Execute(Activity? activity = null , AccessKey? key = null)
    {
        throw new NotImplementedException();
    }
}