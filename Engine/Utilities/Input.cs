using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;

namespace Engine.Utilities;


public static class Input
{
    private static WpfKeyboard keyboard;
    private static WpfMouse mouse;

    public static MouseState MouseState { get; set; }
    public static KeyboardState KeyState { get; set; }

    public static MouseState LastMouseState { get; set; }
    public static KeyboardState LastKeyState { get; set; }
    public static Camera Cam { get; private set; }
    public static void Init(WpfGame game, Camera cam)
    {
        Cam = cam;
        keyboard = new WpfKeyboard(game);
        mouse = new WpfMouse(game);
        mouse.CaptureMouseWithin = false;
        LastKeyState = default;
        LastMouseState = default;
        KeyState = default;
        MouseState = default;
    }

    public static void Update()
    {
        LastMouseState = MouseState;
        LastKeyState = KeyState;

        MouseState = mouse.GetState();
        KeyState = keyboard.GetState();
    }

    public static float GetAxis(Axis axis)
    {
        var result = 0f;
        switch (axis)
        {
            case Axis.Horizontal:
                if (KeyState.IsKeyDown(Keys.D) || KeyState.IsKeyDown(Keys.Right)) 
                    result += 1f;

                if (KeyState.IsKeyDown(Keys.A) || KeyState.IsKeyDown(Keys.Left)) result -= 1f;

                break;
            case Axis.Vertical:
                if (KeyState.IsKeyDown(Keys.W) || KeyState.IsKeyDown(Keys.Up)) result -= 1f;

                if (KeyState.IsKeyDown(Keys.S) || KeyState.IsKeyDown(Keys.Down)) result += 1f;

                break;
        }
        return result;
    }

    public static bool WasPressed(Keys key) => KeyState.IsKeyDown(key) && LastKeyState.IsKeyUp(key);


    public static bool WasPressed(MouseButton button) => button switch
    {
        MouseButton.Left => MouseState.LeftButton == ButtonState.Pressed
                            && LastMouseState.LeftButton == ButtonState.Released,

        MouseButton.Middle => MouseState.MiddleButton == ButtonState.Pressed
                            && LastMouseState.MiddleButton == ButtonState.Released,

        MouseButton.Right => MouseState.RightButton == ButtonState.Pressed
                            && LastMouseState.RightButton == ButtonState.Released,

        _ => false,
    };


    //public static Point GetMousePixelPosition => 
    //    new ((int)(MouseState.X + Cam.Position.X),(int)(MouseState.Y + Cam.Position.Y));

    public static Point GetMouseScreenPosition()
    {
        return Cam.ScreenToWorldSpace(new Vector2(Input.MouseState.X, Input.MouseState.Y)).ToPoint();
    }

    public enum MouseButton
    {
        Left,
        Middle,
        Right,
    }
    public enum Axis
    {
        Horizontal,
        Vertical
    }
}
