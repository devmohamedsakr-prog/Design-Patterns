using System;

namespace GameConsoleFactory.After.Context
{
    // Abstract products
    public interface IInputHandler
    {
        void HandleInput(string input);
        string GetControllerType();
    }

    public interface IGraphicsEngine
    {
        void Render();
        string GetGraphicsAPI();
    }

    public interface IAudioEngine
    {
        void PlaySound(string soundFile);
        string GetAudioFormat();
    }

    // Abstract factory
    public interface IGameConsoleFactory
    {
        IInputHandler CreateInputHandler();
        IGraphicsEngine CreateGraphicsEngine();
        IAudioEngine CreateAudioEngine();
    }

    // PlayStation implementations
    public class PlayStationInput : IInputHandler
    {
        public void HandleInput(string input) => Console.WriteLine($"🎮 PlayStation: DualSense controller input: {input}");
        public string GetControllerType() => "DualSense";
    }

    public class PlayStationGraphics : IGraphicsEngine
    {
        public void Render() => Console.WriteLine("🎨 PlayStation: Rendering with AMD RDNA 2 GPU");
        public string GetGraphicsAPI() => "GNM";
    }

    public class PlayStationAudio : IAudioEngine
    {
        public void PlaySound(string soundFile) => Console.WriteLine($"🔊 PlayStation: Playing {soundFile} with Tempest 3D audio");
        public string GetAudioFormat() => "Tempest 3D";
    }

    // Xbox implementations
    public class XboxInput : IInputHandler
    {
        public void HandleInput(string input) => Console.WriteLine($"🎮 Xbox: Xbox Wireless controller input: {input}");
        public string GetControllerType() => "Xbox Wireless";
    }

    public class XboxGraphics : IGraphicsEngine
    {
        public void Render() => Console.WriteLine("🎨 Xbox: Rendering with AMD RDNA 2 GPU");
        public string GetGraphicsAPI() => "DirectX 12";
    }

    public class XboxAudio : IAudioEngine
    {
        public void PlaySound(string soundFile) => Console.WriteLine($"🔊 Xbox: Playing {soundFile} with Dolby Atmos");
        public string GetAudioFormat() => "Dolby Atmos";
    }

    // Nintendo Switch implementations
    public class NintendoInput : IInputHandler
    {
        public void HandleInput(string input) => Console.WriteLine($"🎮 Nintendo: Joy-Con controller input: {input}");
        public string GetControllerType() => "Joy-Con";
    }

    public class NintendoGraphics : IGraphicsEngine
    {
        public void Render() => Console.WriteLine("🎨 Nintendo: Rendering with NVIDIA Maxwell GPU");
        public string GetGraphicsAPI() => "NVN";
    }

    public class NintendoAudio : IAudioEngine
    {
        public void PlaySound(string soundFile) => Console.WriteLine($"🔊 Nintendo: Playing {soundFile} with stereo audio");
        public string GetAudioFormat() => "Stereo PCM";
    }

    // Concrete factories
    public class PlayStationFactory : IGameConsoleFactory
    {
        public IInputHandler CreateInputHandler() => new PlayStationInput();
        public IGraphicsEngine CreateGraphicsEngine() => new PlayStationGraphics();
        public IAudioEngine CreateAudioEngine() => new PlayStationAudio();
    }

    public class XboxFactory : IGameConsoleFactory
    {
        public IInputHandler CreateInputHandler() => new XboxInput();
        public IGraphicsEngine CreateGraphicsEngine() => new XboxGraphics();
        public IAudioEngine CreateAudioEngine() => new XboxAudio();
    }

    public class NintendoFactory : IGameConsoleFactory
    {
        public IInputHandler CreateInputHandler() => new NintendoInput();
        public IGraphicsEngine CreateGraphicsEngine() => new NintendoGraphics();
        public IAudioEngine CreateAudioEngine() => new NintendoAudio();
    }

    // Factory provider
    public class ConsoleFactoryProvider
    {
        public static IGameConsoleFactory GetFactory(string console)
        {
            return console.ToLower() switch
            {
                "playstation" => new PlayStationFactory(),
                "xbox" => new XboxFactory(),
                "nintendo" => new NintendoFactory(),
                _ => throw new ArgumentException($"Unknown console: {console}")
            };
        }
    }

    // Game engine
    public class GameEngine
    {
        private IInputHandler _input;
        private IGraphicsEngine _graphics;
        private IAudioEngine _audio;

        public GameEngine(IGameConsoleFactory factory)
        {
            _input = factory.CreateInputHandler();
            _graphics = factory.CreateGraphicsEngine();
            _audio = factory.CreateAudioEngine();
        }

        public void RunGame()
        {
            Console.WriteLine($"\n🎮 Game Engine started");
            _input.HandleInput("Jump");
            _graphics.Render();
            _audio.PlaySound("jump.wav");
            Console.WriteLine($"✓ Game running with controller: {_input.GetControllerType()}");
        }
    }
}
