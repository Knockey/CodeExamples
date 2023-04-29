using System;

namespace Tutorial
{
    public class TutorialException : Exception
    {
        public TutorialException(TutorialStageType tutorialStage, string methodName)
            : base($"{tutorialStage} is completed! Check with {methodName} before start!") { }

        public TutorialException()
            : base("Add some stages in tutorial!") { }
    }
}
