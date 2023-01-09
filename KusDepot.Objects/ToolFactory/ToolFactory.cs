namespace KusDepot
{
    /** <include file = 'ToolFactory.xml' path = 'ToolFactory/class[@Name = "ToolFactory"]'/> */
    public static class ToolFactory
    {
        /** <include file = 'ToolFactory.xml' path = 'ToolFactory/class/method[@Name = "Forge"]'/> */
        public static Tool? Forge(List<DataItem>? data , Guid? id , Queue<Object>? inputs , String? name , List<String>? notes , List<Object>? objectives , List<Object>? policies , String? purpose , List<String>? tags)
        {
            return new Tool(data,id,inputs,name,notes,objectives,policies,purpose,tags);
        }

    }
}