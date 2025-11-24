[assembly:KusDepot.CommandContainer(typeof(KusDepot.FabricExams.CommandExam1),"Handle001","Specification Sheet")]
[assembly:KusDepot.CommandContainer(typeof(KusDepot.FabricExams.CommandExam1),"Handle002","Usage...")]

namespace KusDepot.FabricExams;

public class CommandExam1 : Command
{
    public override Guid? Execute(Activity? activity , AccessKey? key = null)
    {
        throw new NotImplementedException();
    }
}