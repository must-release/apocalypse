
public static class ChapterStageCount
{
    public static int GetStageCount(ChapterType chapterType)
    {
        switch(chapterType)
        {
            case ChapterType.Test:
                return 3;
            default:
                return 0;
        }
    }
}