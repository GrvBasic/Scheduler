



namespace DownloadPlugin
{
    public enum TaskType
    {

        //Tasks like read and write files
        HighPriority,

        //Tasks that are dependent to high priority taSK
        MediumPriority,
    
        //Tasks that are dependent to medium priority or high priority or both.
        LowPriority
   
    }
}