using OnixRuntime.Api;
using OnixRuntime.Api.Maths;
using WorldEdit.Tool;

namespace WorldEdit.Tool;

public static class SlotDrawer
{
    public static float StartX = 0.63794f; 
    public static float StartY = 0.9200f;
    public static float Size =  0.03120f;
    
    public static (Vec2 pos, float size) GetTenthSlot(float screenW, float screenH)
    {
        float slotX = 0.847099f - 0.0002806f * screenW;
        float slotY = 0.884802f + 0.00012399f * screenH;
        float slotSize = 0.0762968f - 0.00006194f * screenW;

        return (new Vec2(slotX, slotY), slotSize);
    }



    public static SlotRects GetSlotRects(int nAbove)
    {
        float screenWidth  = Onix.Gui.ScreenSize.X;
        float screenHeight = Onix.Gui.ScreenSize.Y;

        if (!ToolManager.Configuring)
        {
            StartX = WorldEdit.Config.TenthSlotX;
            StartY = WorldEdit.Config.TenthSlotY;
            Size = WorldEdit.Config.TenthSlotSize;
        }
        
        Vec2 baseTopLeft = new Vec2(screenWidth * StartX, screenHeight * StartY);
        Vec2 sizeVec     = new Vec2(screenWidth * Size, screenWidth * Size);

        // Offset upwards by nAbove slots
        Vec2 topLeft = new Vec2(
            baseTopLeft.X,
            baseTopLeft.Y - sizeVec.Y * nAbove
        );

        // Main slot rect
        Rect fullSlot = new Rect(topLeft, topLeft + sizeVec);

        // Expanded slot (1/16 in all directions)
        Vec2 expandFactor     = new Vec2(sizeVec.X / 16f, sizeVec.Y / 16f);
        Vec2 expandedTopLeft  = topLeft - expandFactor;
        Vec2 expandedSize     = sizeVec + expandFactor * 2f;
        Rect expandedSlot     = new Rect(expandedTopLeft, expandedTopLeft + expandedSize);

        // Sixteenth width
        float sixteenthWidth = sizeVec.X / 16f;

        // Leftmost 1/16
        Vec2 leftSixteenthTl = topLeft;
        Vec2 leftSixteenthBr = new Vec2(topLeft.X + sixteenthWidth, topLeft.Y + sizeVec.Y);
        Rect leftSixteenth = new Rect(leftSixteenthTl, leftSixteenthBr);

        // Rightmost 1/16
        Vec2 rightSixteenthTl = new Vec2(topLeft.X + sizeVec.X - sixteenthWidth, topLeft.Y);
        Vec2 rightSixteenthBr = new Vec2(topLeft.X + sizeVec.X, topLeft.Y + sizeVec.Y);
        Rect rightSixteenth = new Rect(rightSixteenthTl, rightSixteenthBr);

        
        // Text slot
        float horizontalPadding = sizeVec.X / 16f;

        Vec2 textSlotTopLeft = new Vec2(
            expandedTopLeft.X + expandedSize.X + horizontalPadding,
            topLeft.Y
        );

        Vec2 textSlotBottomRight = new Vec2(
            textSlotTopLeft.X + sizeVec.X * 2f,
            textSlotTopLeft.Y + sizeVec.Y
        );

        Rect textSlot = new Rect(textSlotTopLeft, textSlotBottomRight);
        

        Vec2 shrinkFactor = new Vec2(sizeVec.X / 16f, sizeVec.Y / 16f);
        Vec2 shrunkenTopLeft = topLeft + shrinkFactor;

        Vec2 bit = new Vec2(0,0);
        Vec2 itemPos = shrunkenTopLeft + bit;

        return new SlotRects {
            FullSlot      = fullSlot,
            ExpandedSlot  = expandedSlot,
            LeftSixteenth = leftSixteenth,
            RightSixteenth= rightSixteenth,
            ItemPos       = itemPos,
            TextSlot      = textSlot
        };
    }

}
