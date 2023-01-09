namespace KusDepot
{
    /** <include file = 'Clutch.xml' path = 'Clutch/class[@Name = "Clutch"]'/> */
    public class Clutch
    {
        /** <include file = 'Clutch.xml' path = 'Clutch/class[@Name = "Clutch"]/field[@Name = "Stuff"]'/> */
        private ConcurrentBag<DataItem>? Stuff;

        /** <include file = 'Clutch.xml' path = 'Clutch/class[@Name = "Clutch"]/field[@Name = "Tools"]'/> */
        private ConcurrentBag<Tool>? Tools;

        /** <include file = 'Clutch.xml' path = 'Clutch/class[@Name = "Clutch"]/constructor[@Name = "ParameterlessConstructor"]'/> */
        public Clutch() { }

        /** <include file = 'Clutch.xml' path = 'Clutch/class[@Name = "Clutch"]/constructor[@Name = "Constructor"]'/> */
        public Clutch(List<DataItem>? stuff , List<Tool>? tools)
        {
            try
            {
                if(stuff is not null) { this.Stuff = new ConcurrentBag<DataItem>(stuff); }
                if(tools is not null) { this.Tools = new ConcurrentBag<Tool>(tools); }
            }
            catch ( Exception ie ) { throw new Exception("KusDepot.Clutch.Constructor",ie); }
        }

    }

}