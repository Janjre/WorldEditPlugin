using System.Runtime.InteropServices.Marshalling;
using OnixRuntime.Api;
using OnixRuntime.Api.Maths;

namespace WorldEdit;

public static class AxisCalculator
{
    public enum AxisEnum { X, Y, Z, Facing }
    public enum DirectionEnum { XPos, YPos, ZPos, XNeg, YNeg, ZNeg, Facing }
    public enum FaceEnum { XZ, XY, ZY, Facing }

    public enum BasicAxis {X,Y,Z}
    
    public class Axis
    {
        public BasicAxis DirectionAxis;
        public Vec3 DirectionVector;
        
        public Axis(AxisEnum axis)
        {
            if (axis == AxisEnum.Facing)
            {
                Vec3 playerFacingGlobal = Onix.LocalPlayer.ForwardPosition(1);
                Vec3 playerFacingLocal = playerFacingGlobal - Onix.LocalPlayer.Position.WithY(Onix.LocalPlayer.Position.Y + 1.62f);
                
                playerFacingLocal.X = Math.Abs(playerFacingLocal.X);
                playerFacingLocal.Y = Math.Abs(playerFacingLocal.Y);
                playerFacingLocal.Z = Math.Abs(playerFacingLocal.Z);

                float biggestValue = 0f;
                BasicAxis biggestDirection = BasicAxis.X;
                if (playerFacingLocal.X > biggestValue) { biggestDirection = BasicAxis.X; biggestValue = playerFacingLocal.X; }
                if (playerFacingLocal.Y > biggestValue) { biggestDirection = BasicAxis.Y; biggestValue = playerFacingLocal.Y; }
                if (playerFacingLocal.Z > biggestValue) { biggestDirection = BasicAxis.Z; biggestValue = playerFacingLocal.Z; }

                DirectionAxis = biggestDirection;
                
                if (DirectionAxis == BasicAxis.X) {DirectionVector = new Vec3(1, 0, 0);}
                if (DirectionAxis == BasicAxis.Y) {DirectionVector = new Vec3(0, 1, 0);}
                if (DirectionAxis == BasicAxis.Z) {DirectionVector = new Vec3(0, 0, 1);}
            }
            
            if (axis == AxisEnum.X) {DirectionVector = new Vec3(1, 0, 0);}
            if (axis == AxisEnum.Y) {DirectionVector = new Vec3(0, 1, 0);}
            if (axis == AxisEnum.Z) {DirectionVector = new Vec3(0, 0, 1);}
            
        }
    }
    
    public class Direction
    {
    
        public BasicAxis DirectionAxis;
        public bool FacingDirection;
        public Vec3 DirectionVector;
        
        
        public Direction(DirectionEnum direction)
        {
            if (direction == DirectionEnum.Facing)
            {
                Vec3 playerFacingGlobal = Onix.LocalPlayer.ForwardPosition(1);
                Vec3 playerFacingLocal = playerFacingGlobal - Onix.LocalPlayer.Position.WithY(Onix.LocalPlayer.Position.Y + 1.62f);

                Vec3 playerFacingLocalAbsolute = playerFacingLocal;
                playerFacingLocalAbsolute.X = Math.Abs(playerFacingLocalAbsolute.X);
                playerFacingLocalAbsolute.Y = Math.Abs(playerFacingLocalAbsolute.Y);
                playerFacingLocalAbsolute.Z =  Math.Abs(playerFacingLocalAbsolute.Z);

                float biggestValue = 0f;
                bool biggestValueDirection = false;
                BasicAxis biggestDirection = BasicAxis.X;
                if (playerFacingLocalAbsolute.X > biggestValue) { biggestDirection = BasicAxis.X; biggestValue = playerFacingLocalAbsolute.X; biggestValueDirection=playerFacingLocal.X > 0;}
                if (playerFacingLocalAbsolute.Y > biggestValue) { biggestDirection = BasicAxis.Y; biggestValue = playerFacingLocalAbsolute.Y; biggestValueDirection=playerFacingLocal.Y > 0;}
                if (playerFacingLocalAbsolute.Z > biggestValue) { biggestDirection = BasicAxis.Z; biggestValue = playerFacingLocalAbsolute.Z; biggestValueDirection=playerFacingLocal.Z > 0;}
                

                DirectionAxis = biggestDirection;
                FacingDirection = biggestValueDirection;
                
                if (DirectionAxis == BasicAxis.X) {DirectionVector = new Vec3(biggestValueDirection ? 1 : -1, 0, 0);}
                if (DirectionAxis == BasicAxis.Y) {DirectionVector = new Vec3(0, biggestValueDirection ? 1 : -1, 0);}
                if (DirectionAxis == BasicAxis.Z) {DirectionVector = new Vec3(0, 0, biggestValueDirection ? 1 : -1);}
            }
            
            if (direction == DirectionEnum.XPos) {DirectionVector = new Vec3(1, 0, 0);}
            if (direction == DirectionEnum.YPos) {DirectionVector = new Vec3(0, 1, 0);}
            if (direction == DirectionEnum.ZPos) {DirectionVector = new Vec3(1, 0, 1);}
            if (direction == DirectionEnum.XNeg) {DirectionVector = new Vec3(-1, 0, 0);}
            if (direction == DirectionEnum.YNeg) {DirectionVector = new Vec3(0, -1, 0);}
            if (direction == DirectionEnum.ZNeg) {DirectionVector = new Vec3(1, 0, -1);}
            
            
        }
        
    }
    
    
    
}