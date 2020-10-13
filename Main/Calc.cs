﻿using System;
using System.Collections.Generic;
using System.Linq;

using PPR.Main.Levels;

namespace PPR.Main {
    public static class Calc {
        public static float StepsToMilliseconds(float steps) => StepsToMilliseconds(steps, Map.currentLevel.speeds);
        public static float StepsToMilliseconds(float steps, List<LevelSpeed> sortedSpeeds) {
            float useSteps = steps;

            int speedIndex = 0;
            for(int i = 0; i < sortedSpeeds.Count; i++)
                if(sortedSpeeds[i].step <= useSteps) speedIndex = i;
            float time = 0;
            for(int i = 0; i <= speedIndex; i++)
                if(i == speedIndex)
                    time += (useSteps - sortedSpeeds[i].step) *
                            (sortedSpeeds[i].speed == 0 ? 0 : 60000f / Math.Abs(sortedSpeeds[i].speed));
                else
                    time += (sortedSpeeds[i + 1].step - sortedSpeeds[i].step) *
                            (sortedSpeeds[i].speed == 0 ? 0 : 60000f / Math.Abs(sortedSpeeds[i].speed));
            return time;
        }
        
        public static float MillisecondsToSteps(float time) => MillisecondsToSteps(time, Map.currentLevel.speeds);
        public static float MillisecondsToSteps(float time, List<LevelSpeed> sortedSpeeds) {
            float useTime = time;

            int speedIndex = 0;
            for(int i = 0; i < sortedSpeeds.Count; i++)
                if(StepsToMilliseconds(sortedSpeeds[i].step, sortedSpeeds) <= useTime) speedIndex = i;
                else break;
            float steps = 0;
            for(int i = 0; i <= speedIndex; i++)
                if(i == speedIndex)
                    steps += sortedSpeeds[i].speed == 0 ? 0 : useTime / (60000f / Math.Abs(sortedSpeeds[i].speed));
                else {
                    int stepsIncrement = sortedSpeeds[i + 1].step - sortedSpeeds[i].step;
                    steps += stepsIncrement;
                    useTime -= stepsIncrement *
                               (sortedSpeeds[i].speed == 0 ? 0 : 60000f / Math.Abs(sortedSpeeds[i].speed));
                }

            return steps;
        }
        
        public static float StepsToOffset(float steps) => StepsToOffset(steps, Map.currentLevel.speeds);
        public static float StepsToOffset(float steps, List<LevelSpeed> sortedSpeeds) {
            float useSteps = steps;

            int speedIndex = 0;
            for(int i = 0; i < sortedSpeeds.Count; i++)
                if(sortedSpeeds[i].step <= useSteps) speedIndex = i;
                else break;
            float offset = 0;
            for(int i = 0; i <= speedIndex; i++)
                if(i == speedIndex) offset += useSteps * MathF.Sign(sortedSpeeds[i].speed);
                else {
                    int stepsDecrement = sortedSpeeds[i + 1].step - sortedSpeeds[i].step;
                    offset += stepsDecrement * MathF.Sign(sortedSpeeds[i].speed);
                    useSteps -= stepsDecrement;
                }

            return offset;
        }
        
        public static float OffsetToSteps(float offset, int directionLayer) => OffsetToSteps(offset, directionLayer, Map.currentLevel.speeds);
        public static float OffsetToSteps(float offset, int directionLayer, List<LevelSpeed> sortedSpeeds) {
            for(int i = sortedSpeeds.Count - 1; i >= 0; i--) {
                float currentOffset = StepsToOffset(sortedSpeeds[i].step);
                if(currentOffset <= offset && StepsToDirectionLayer(sortedSpeeds[i].step) == directionLayer)
                    return sortedSpeeds[i].step + offset - currentOffset;
            }

            return float.NaN;
        }
        
        public static int StepsToDirectionLayer(float steps) => StepsToDirectionLayer(steps, Map.currentLevel.speeds);
        public static int StepsToDirectionLayer(float steps, List<LevelSpeed> sortedSpeeds) {
            int speedIndex = 0;
            for(int i = 0; i < sortedSpeeds.Count; i++)
                if(sortedSpeeds[i].step <= steps) speedIndex = i;
                else break;
            int directionLayer = 0;
            for(int i = 1; i <= speedIndex; i++)
                if(MathF.Sign(sortedSpeeds[i].speed) != MathF.Sign(sortedSpeeds[i - 1].speed)) directionLayer++;
            return directionLayer;
        }
        
        public static int GetBPMAtStep(int step, IEnumerable<LevelSpeed> sortedSpeeds) {
            int bpm = 0;
            foreach(LevelSpeed speed in sortedSpeeds)
                if(speed.step <= step) bpm = speed.speed;
                else break;
            return bpm;
        }
        
        public static IEnumerable<int> GetBPMBetweenSteps(int start, int end, IEnumerable<LevelSpeed> sortedSpeeds) =>
            from speed in sortedSpeeds where speed.step > start && speed.step < end select speed.speed;
        
        public static List<LevelSpeed> GetSpeedsBetweenSteps(int start, int end, List<LevelSpeed> sortedSpeeds) =>
            sortedSpeeds.FindAll(speed => speed.step >= start && speed.step <= end);
        
        public static float GetDifficulty(IEnumerable<LevelObject> objects, List<LevelSpeed> sortedSpeeds,
            int lengthMins) => GetDifficulty(
            objects.Select(obj => new LightLevelObject(obj.character, obj.step)).ToList(), sortedSpeeds,
                lengthMins);
        public static float GetDifficulty(IReadOnlyCollection<LightLevelObject> lightObjects, List<LevelSpeed> sortedSpeeds,
            int lengthMins) {
            if (lightObjects.Count == 0 || sortedSpeeds.Count == 0) return 0f;

            List<LightLevelObject> sortedObjects = new List<LightLevelObject>(lightObjects);
            sortedObjects.Sort((obj1, obj2) => obj1.step.CompareTo(obj2.step));
            for(int i = 1; i < sortedObjects.Count; i++)
                if(sortedObjects[i].character == LevelObject.HoldChar)
                    sortedObjects.RemoveAt(i - 1);
            sortedObjects = sortedObjects.FindAll(obj => obj.character != LevelObject.HoldChar);

            List<float> diffFactors = new List<float>();
            
            List<float> speeds = new List<float>();
            List<float> bpm = new List<float>();
            
            List<LightLevelObject>[] objects = {
                new List<LightLevelObject>(),
                new List<LightLevelObject>(),
                new List<LightLevelObject>(),
                new List<LightLevelObject>(),
                new List<LightLevelObject>(),
                new List<LightLevelObject>(),
                new List<LightLevelObject>(),
                new List<LightLevelObject>(),
                new List<LightLevelObject>(),
                new List<LightLevelObject>(),
                new List<LightLevelObject>(),
                new List<LightLevelObject>()
            };
            foreach(LightLevelObject obj in sortedObjects)
                objects[(GetXPosForCharacter(obj.character) - 6) / 12].Add(obj);
            foreach(List<LightLevelObject> objs in objects)
                for(int i = 1; i < objs.Count; i++) {
                    LightLevelObject prevObj = objs[i - 1];
                    LightLevelObject currObj = objs[i];
                    int startBPM = Math.Abs(GetBPMAtStep(prevObj.step, sortedSpeeds));
                    int endBPM = Math.Abs(GetBPMAtStep(currObj.step, sortedSpeeds));
                    int currStep = prevObj.step - startBPM / 600;
                    int endStep = currObj.step + endBPM / 600;
                    float time = 0;
                    int currBPM = startBPM;
                    foreach(LevelSpeed speed in GetSpeedsBetweenSteps(prevObj.step, currObj.step,
                        sortedSpeeds)) {
                        time += 60f / currBPM * (speed.step - currStep);
                        currStep = speed.step;
                        currBPM = Math.Abs(speed.speed);
                    }
                    time += 60f / endBPM * (endStep - currStep);
                    float distance = GetPhysicalKeyDistance(currObj.character, prevObj.character);
                    speeds.Add(distance + 1f);
                    if(time != 0f) speeds.Add(1f / time);
                }

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach(LevelSpeed speed in sortedSpeeds) bpm.Add(Math.Abs(speed.speed) / 60f);
            
            diffFactors.Add(speeds.Count > 0 ? speeds.Average() : 0f);
            diffFactors.Add(bpm.Average());
            diffFactors.Add(lengthMins);

            return diffFactors.Count > 0 ? diffFactors.Average() : 0f;
        }
        
        public static int GetXPosForCharacter(char character) {
            character = char.ToLower(character);
            int x = 0;
            int xLineOffset = 0;
            int mul = 90 / LevelObject.lines.Select(line => line.Length).Max();
            foreach(string line in LevelObject.lines) {
                if(line.Contains(character)) {
                    x = (line.IndexOf(character) + 1) * (mul - 1) + xLineOffset * mul / 3;
                    break;
                }
                xLineOffset++;
            }
            return x;
        }
        public static float GetPhysicalKeyDistance(char leftChar, char rightChar) {
            int leftX = GetXPosForCharacter(leftChar);
            int rightX = GetXPosForCharacter(rightChar);
            int leftY = 0;
            int rightY = 0;
            int lineOffset = 0;
            foreach(string line in LevelObject.lines) {
                if(line.Contains(leftChar)) leftY = lineOffset;
                if(line.Contains(rightChar)) rightY = lineOffset;
                lineOffset++;
            }
            return MathF.Sqrt((leftX - rightX) / 6f * ((leftX - rightX) / 6f) + (leftY - rightY) * (leftY - rightY));
        }
    }
}
