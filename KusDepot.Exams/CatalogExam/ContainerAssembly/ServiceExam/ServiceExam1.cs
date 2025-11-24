[assembly:KusDepot.ServiceContainer(typeof(KusDepot.FabricExams.ServiceExam1))]

namespace KusDepot.FabricExams;

public class ServiceExam1 : Tool , ISocketManager { }