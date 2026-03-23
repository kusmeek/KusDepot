[assembly:KusDepot.ServiceContainer(typeof(KusDepot.Exams.ServiceExam1),"Usage...")]

namespace KusDepot.Exams;

public class ServiceExam1 : Tool , ISocketManager { }