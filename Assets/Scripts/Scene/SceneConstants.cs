
public static class ChapterStageCount
{
    public static int GetStageCount(ChapterType chapterType)
    {
        switch(chapterType)
        {
            case ChapterType.Test:
                return 3;
            case ChapterType.Prologue:
                return 2;
            default:
                return 0;
        }
    }

    public static bool IsStageIndexValid(ChapterType chapter, int stage)
    {
        return (0 < stage) && (stage <= GetStageCount(chapter));
    }
}