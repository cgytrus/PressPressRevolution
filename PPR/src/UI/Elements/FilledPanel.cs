﻿using System.Collections.Generic;

using PER.Abstractions.Renderer;

using PRR;

using SFML.Graphics;
using SFML.System;

namespace PPR.UI.Elements {
    public class FilledPanel : Panel {
        public override string type => "filledPanel";
        
        private Color color => GetColor("color");
        
        public FilledPanel(string id, List<string> tags, Vector2i? position, Vector2i? size, Vector2f? anchor,
            Element parent) : base(id, tags, position, size, anchor, parent) { }

        public override void Draw() {
            Color color = this.color;
            for(int x = bounds.min.X; x < bounds.max.X; x++) {
                for(int y = bounds.min.Y; y < bounds.max.Y; y++) {
                    Vector2i pos = new Vector2i(x, y);
                    RenderCharacter character = new RenderCharacter('\0', color, color);
                    (pos, character) = ApplyAnimations(pos, character);
                    Core.renderer.DrawCharacter(pos, character);
                }
            }
        }
    }
}