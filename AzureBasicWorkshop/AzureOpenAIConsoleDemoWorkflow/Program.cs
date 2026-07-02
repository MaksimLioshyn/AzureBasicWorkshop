namespace AzureOpenAIConsoleDemoWorkflow
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var workflowGuide = new WorkflowGuide();
            Console.WriteLine(workflowGuide.GetSummary());
        }
    }
}