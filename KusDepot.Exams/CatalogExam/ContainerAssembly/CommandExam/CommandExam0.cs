[assembly:KusDepot.CommandContainer(typeof(KusDepot.FabricExams.CommandExam0),"Handle001","Specification Sheet")]
[assembly:KusDepot.CommandContainer(typeof(KusDepot.FabricExams.CommandExam0),"Handle002","Usage...")]

namespace KusDepot.FabricExams;

public class CommandExam0 : Command
{
    public override Guid? Execute(Activity? activity , AccessKey? key = null)
    {
        throw new NotImplementedException();
    }
}