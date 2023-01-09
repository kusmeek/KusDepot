namespace KusDepot
{
    /** <include file = 'Formation.xml' path = 'Formation/class[@Name = "Formation"]'/> */
    public class Formation
    {
        /** <include file = 'Formation.xml' path = 'Formation/class[@Name = "Formation"]/field[@Name = "Authority"]'/> */
        public Tool? Authority;

        /** <include file = 'Formation.xml' path = 'Formation/class[@Name = "Formation"]/field[@Name = "Extension"]'/> */
        public dynamic? Extension;

        /** <include file = 'Formation.xml' path = 'Formation/class[@Name = "Formation"]/field[@Name = "Name"]'/> */
        public String? Name;

        /** <include file = 'Formation.xml' path = 'Formation/class[@Name = "Formation"]/field[@Name = "Notes"]'/> */
        public List<String>? Notes;

        /** <include file = 'Formation.xml' path = 'Formation/class[@Name = "Formation"]/field[@Name = "Operators"]'/> */
        public ConcurrentBag<Tool>? Operators;

        /** <include file = 'Formation.xml' path = 'Formation/class[@Name = "Formation"]/field[@Name = "Repository"]'/> */
        public ConcurrentBag<DataItem>? Repository;

        /** <include file = 'Formation.xml' path = 'Formation/class[@Name = "Formation"]/field[@Name = "Subformations"]'/> */
        public ConcurrentBag<Formation>? Subformations;

        /** <include file = 'Formation.xml' path = 'Formation/class[@Name = "Formation"]/field[@Name = "Tags"]'/> */
        public List<String>? Tags;

        /** <include file = 'Formation.xml' path = 'Formation/class[@Name = "Formation"]/constructor[@Name = "ParameterlessConstructor"]'/> */
        public Formation() { }
    }

}