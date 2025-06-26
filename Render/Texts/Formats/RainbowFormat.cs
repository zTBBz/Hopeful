using Microsoft.Xna.Framework;
using System;

namespace Hopeful.Render.Texts.Formats;

public class RainbowFormat
{
    [Inject]
    private readonly TextFormatStore _store = null!;

    public void Setup()
    {
        var rainbow = TextFormatBuilder.Setup()
            .WithParameter("speed", 1f)
            .WithUpdate((format, segment, deltaTime) =>
            {
                var hue = (segment.Position.X * 0.05f + deltaTime * format.GetParameterValue<float>("speed", segment)) % 1.0f;
                segment.Color = ColorHelper.FromHsv(hue, 1f, 1f);
            }).Save();
        
        _store.AddTextFormat("Rainbow", rainbow);


        var bounce = TextFormatBuilder.Setup()
            .WithParameter("speed", 0.5f)
            .WithParameter("height", 5f)
            .WithUpdate((format, segment, deltaTime) =>
            {
                var bounce = (float)Math.Sin(deltaTime * format.GetParameterValue<float>("speed", segment)) *
                             format.GetParameterValue<float>("height", segment);
                segment.Position = new Vector2(segment.Position.X, segment.Position.Y - (int)bounce); ;
            }).Save();


        var typing = TextFormatBuilder.Setup()
            .WithParameter("speed", 10f)
            .WithUpdate((format, segment, deltaTime) =>
            {



            }).Save();
    }
}
