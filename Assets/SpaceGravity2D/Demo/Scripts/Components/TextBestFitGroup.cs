using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceGravity2D.Demo
{
    /// <summary>
    /// Component for making group of UI texts use same font size (calculated in runtime), when BestFit option is active.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    [ExecuteInEditMode]
    public class TextBestFitGroup : MonoBehaviour
    {
        public enum GroupFitType
        {
            MinFontSize,
            MaxFontSize
        }

        public GroupFitType FitType = GroupFitType.MinFontSize;
        public Text[] TargetTexts = new Text[0];

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            if (TargetTexts.Length > 1)
            {
                var bestFontSize = TargetTexts[0].cachedTextGenerator.fontSizeUsedForBestFit;
                for (int i = 1; i < TargetTexts.Length; i++)
                {
                    if (FitType == GroupFitType.MinFontSize)
                    {
                        if (TargetTexts[i].cachedTextGenerator.fontSizeUsedForBestFit < bestFontSize)
                        {
                            bestFontSize = TargetTexts[i].cachedTextGenerator.fontSizeUsedForBestFit;
                        }
                    }
                    else if (FitType == GroupFitType.MaxFontSize)
                    {
                        if (TargetTexts[i].cachedTextGenerator.fontSizeUsedForBestFit > bestFontSize)
                        {
                            bestFontSize = TargetTexts[i].cachedTextGenerator.fontSizeUsedForBestFit;
                        }
                    }
                }
                for (int i = 0; i < TargetTexts.Length; i++)
                {
                    TargetTexts[i].resizeTextMaxSize = bestFontSize;
                }
            }
        }
    }
}