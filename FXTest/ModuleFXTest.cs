using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FXTest
{
    public class ModuleFXTest : PartModule
    {
        [KSPField(guiActive = true, guiName = "Test FX")]
        [UI_Toggle(scene = UI_Scene.Flight)]
        public bool playFX = false;

        [KSPField(guiName = "Current Effect")]
        public string currentEffectName;

        [KSPField(guiName = "Test FX Power")]
        [UI_FloatRange(minValue = 0f, maxValue = 1f, scene = UI_Scene.Flight)]
        public float fxPower = 0f;

        private List<string> effects;
        private int currentIndex = 0;

        private BaseField currentEffectNameField;
        private BaseField fxPowerField;
        private BaseEvent nextEffectEvent;

        [KSPEvent(guiName = "Next Effect")]
        public void NextEffect()
        {
            currentIndex += 1;
            if (currentIndex >= effects.Count)
                currentIndex = 0;

            currentEffectName = effects[currentIndex];
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            if (state == StartState.Editor)
                return;

            effects = new List<string>();

            // No direct access to list of effects
            // Have to save config node and read off names of effects
            ConfigNode node = new ConfigNode("EFFECTS");
            part.Effects.OnSave(node);
            Debug.Log("Adding effects");
            for (int i = 0; i < node.nodes.Count; i++)
            {
                effects.Add(node.nodes[i].name);
                Debug.Log("Adding effect " + node.nodes[i].name);
            }

            currentEffectName = effects[currentIndex];

            currentEffectNameField = Fields["currentEffectName"];
            fxPowerField = Fields["fxPower"];
            nextEffectEvent = Events["NextEffect"];
        }

        public void Update()
        {
            if (!HighLogic.LoadedSceneIsFlight)
                return;
            
            currentEffectNameField.guiActive = playFX;
            fxPowerField.guiActive = playFX;
            nextEffectEvent.guiActive = playFX;

            if (playFX)
                part.Effect(effects[currentIndex], fxPower);
        }
    }
}
