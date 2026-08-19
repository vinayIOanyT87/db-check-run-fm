namespace InProcLogging
{
    public interface IMailbox
    {
        Message getNextMessage();
        int messageCount();
        bool registerMailbox(string aName);
        bool deregisterMailbox();
        bool sendMessage(Message aMsg, string aTo);
    }
}