namespace KusDepot
{
    /** <include file = 'Work.xml' path = 'Work/class[@Name = "Work"]'/> */
    public class Work
    {
        /** <include file = 'Work.xml' path = 'Work/class[@Name = "Work"]/field[@Name = "Assets"]'/> */
        private ConcurrentBag<DataItem>? Assets;

        /** <include file = 'Work.xml' path = 'Work/class[@Name = "Work"]/field[@Name = "Groups"]'/> */
        private ConcurrentBag<Formation>? Groups;

        /** <include file = 'Work.xml' path = 'Work/class[@Name = "Work"]/field[@Name = "Operations"]'/> */
        private ConcurrentBag<Tool>? Operations;

        /** <include file = 'Work.xml' path = 'Work/class[@Name = "Work"]/constructor[@Name = "ParameterlessConstructor"]'/> */
        public Work() { }

        /** <include file = 'Work.xml' path = 'Work/class[@Name = "Work"]/constructor[@Name = "Constructor"]'/> */
        public Work(List<DataItem>? assets , List<Formation>? groups , List<Tool>? operations)
        {
            try
            {
                if(assets is not null) { this.Assets = new ConcurrentBag<DataItem>(assets); }
                if(groups is not null) { this.Groups = new ConcurrentBag<Formation>(groups); }
                if(operations is not null) { this.Operations = new ConcurrentBag<Tool>(operations); }
            }
            catch ( Exception ie ) { throw new Exception("KusDepot.Work.Constructor",ie); }
        }

    }

}