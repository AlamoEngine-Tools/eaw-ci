namespace EawXBuild.Steam
{
    public class WorkshopItemPublishResult
    {
        public WorkshopItemPublishResult(ulong itemId, PublishResult result)
        {
            ItemId = itemId;
            Result = result;
        }

        public ulong ItemId { get; }
        public PublishResult Result { get; }
    }
}