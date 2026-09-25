using System.Collections.Generic;
using RespondX.Models;

namespace RespondX.Helpers
{
    public static class QuizQuestionBank
    {
        public static List<QuestionItem> GetQuestions(int quizId)
        {
            switch (quizId)
            {
                case 1:
                    return GetEmergencyResponseQuestions();
                case 2:
                    return GetCprAndFirstAidQuestions();
                case 3:
                    return GetEmergencyCommunicationQuestions();
                default:
                    return new List<QuestionItem>();
            }
        }

        private static List<QuestionItem> GetEmergencyResponseQuestions()
        {
            return new List<QuestionItem>
            {
                Q(1, "What should you do before approaching an emergency scene?", 1,
                    "Immediately move the injured person", "Check for hazards and make sure the scene is safe", "Ask bystanders to gather around", "Search the person's belongings"),
                Q(2, "You see a live electrical wire near an injured person. What is the safest response?", 3,
                    "Step over the wire carefully", "Pull the person away", "Pour water on the wire", "Stay back, warn others, and contact emergency services"),
                Q(3, "When calling emergency services, which information is most useful first?", 0,
                    "The exact location and the type of emergency", "Your opinion about who caused it", "A description of everyone watching", "A guess about how long help will take"),
                Q(4, "If several bystanders are present, how should you ask someone to call for help?", 2,
                    "Shout generally for anyone to call", "Assume another person has already called", "Point to one person, give the task, and confirm they are doing it", "Ask everyone to call separately"),
                Q(5, "A responsive person needs first aid. What should you do before providing care?", 1,
                    "Begin treatment without speaking", "Introduce yourself, explain what you want to do, and ask permission", "Ask them to stand up immediately", "Give them food or water"),
                Q(6, "An unconscious person cannot answer you. Which action should you avoid?", 0,
                    "Giving them food, drink, or medicine by mouth", "Calling emergency services when needed", "Watching for changes in breathing", "Following dispatcher instructions"),
                Q(7, "An adult is unresponsive and only gasping. What is an appropriate next step?", 3,
                    "Wait several minutes to see if it stops", "Give the person water", "Leave to find their family", "Call emergency services, get an AED if available, and begin CPR according to your training or dispatcher guidance"),
                Q(8, "A person may have a head, neck, or spinal injury. What should you do?", 2,
                    "Ask them to turn their head", "Help them walk to a chair", "Keep them still unless there is immediate danger, and follow emergency guidance", "Move them to test whether they can stand"),
                Q(9, "Why use gloves or another appropriate barrier when giving first aid?", 1,
                    "They make the responder look official", "They help reduce contact with blood or other potentially infectious material", "They replace hand washing", "They are needed only after care is complete"),
                Q(10, "When may moving an injured person be necessary?", 0,
                    "When staying in place presents an immediate danger", "Whenever the person is uncomfortable", "To make room for bystanders", "Whenever the person cannot answer questions"),
                Q(11, "While waiting for emergency responders, what should you do if it is safe?", 3,
                    "Leave the person alone to look for supplies", "Stop checking once help is called", "Move the person repeatedly", "Continue monitoring them and provide care within your training"),
                Q(12, "Which location description best helps responders find an incident?", 2,
                    "Somewhere near the main area", "The place where we usually meet", "Building name, entrance, floor or room, and a nearby landmark", "The address of a nearby business only"),
                Q(13, "A crowd is blocking an ambulance entrance. What is the best action?", 1,
                    "Let the crowd decide where to stand", "Ask people to clear the access route and direct them to a safe area", "Block the route with equipment", "Send more people to the entrance"),
                Q(14, "You hear an unverified report about a second hazard. What should you do?", 0,
                    "Treat it as unconfirmed and report it through the appropriate official channel", "Post it publicly as a confirmed fact", "Ignore all further information", "Change the evacuation route without telling anyone"),
                Q(15, "What belongs in an incident note or handoff?", 2,
                    "Rumors about the cause", "Personal criticism of other helpers", "Observed facts, times, changes, and actions taken", "Only details that support your first guess"),
                Q(16, "A situation is beyond your first-aid training. What should you do?", 3,
                    "Try an unfamiliar procedure", "Give medication you found nearby", "Ignore the person's condition", "Call for qualified help and follow dispatcher instructions"),
                Q(17, "How should you speak to a responsive person who is frightened?", 1,
                    "Use complex medical terms", "Identify yourself, speak calmly, and explain what is happening", "Promise that nothing bad can happen", "Ask several people to question them at once"),
                Q(18, "During an evacuation, which direction should people follow?", 0,
                    "The route and instructions provided by authorized responders", "The route with the most people", "Any route that passes the hazard", "A route shared in an unverified message"),
                Q(19, "Emergency responders arrive and take over care. What should you tell them?", 3,
                    "Only your name", "Your theory about the person's diagnosis", "A long account of unrelated events", "What you observed, when it happened, and what care was given"),
                Q(20, "After an incident, what should a responder do according to their organization's procedures?", 2,
                    "Share private details on social media", "Discard all used supplies without reporting", "Complete the required report and replace used supplies", "Change the incident record to make it shorter")
            };
        }

        private static List<QuestionItem> GetCprAndFirstAidQuestions()
        {
            return new List<QuestionItem>
            {
                Q(1, "For a teen or adult who suddenly collapses, which action is part of Hands-Only CPR?", 1,
                    "Give food or water", "Call emergency services and push hard and fast in the center of the chest", "Wait for the person to wake up", "Use an AED without turning it on"),
                Q(2, "What chest-compression rate does the American Heart Association recommend for adult CPR?", 3,
                    "20 to 40 per minute", "50 to 70 per minute", "140 to 160 per minute", "100 to 120 per minute"),
                Q(3, "Where should a bystander place their hands for adult chest compressions?", 0,
                    "In the center of the chest", "Over the stomach", "At the side of the neck", "On the lower ribs"),
                Q(4, "What should happen to the chest between compressions?", 2,
                    "Keep constant downward pressure", "Lift the person from the floor", "Allow the chest to return to its normal position", "Pause for several seconds after every push"),
                Q(5, "What surface is preferred for CPR compressions?", 1,
                    "A soft bed", "A firm, flat surface", "A chair with wheels", "A sloped surface"),
                Q(6, "When another bystander is available during CPR, what is a useful task to assign?", 0,
                    "Ask them to call emergency services and bring an AED", "Ask them to move the person repeatedly", "Ask them to leave without a task", "Ask them to give the person a drink"),
                Q(7, "When an AED arrives, what should the rescuer do?", 3,
                    "Ignore its instructions and continue without checking", "Wait until EMS arrives before opening it", "Use it only if the person is awake", "Turn it on and follow its voice and visual prompts"),
                Q(8, "The AED is analyzing the person's heart rhythm. What should bystanders do?", 2,
                    "Hold the person's shoulders", "Continue touching the person", "Make sure nobody is touching the person", "Move the AED away"),
                Q(9, "After an AED delivers a shock, what should rescuers do?", 1,
                    "Stop helping and wait", "Resume CPR when instructed by the AED", "Remove the AED pads immediately", "Give the person something to drink"),
                Q(10, "Hands-Only CPR guidance is mainly intended for which situation?", 0,
                    "A teen or adult who suddenly collapses", "Every infant emergency", "A responsive person with a minor cut", "Any person who is breathing normally"),
                Q(11, "When should CPR be stopped or paused?", 3,
                    "Whenever a bystander asks to take a photo", "Every minute to check for a pulse", "When an AED is nearby but not yet turned on", "When the person shows signs of life, the scene becomes unsafe, you are exhausted, or trained responders take over"),
                Q(12, "What is the Red Cross emergency action sequence for first aid?", 2,
                    "Care, Check, Call", "Call, Care, Check", "Check, Call, Care", "Wait, Move, Treat"),
                Q(13, "When checking an unresponsive person for breathing, how long should the check take?", 1,
                    "At least one minute", "No more than 10 seconds", "Five full minutes", "Until another person arrives"),
                Q(14, "What is an appropriate first-aid action for life-threatening external bleeding?", 0,
                    "Call emergency services and apply firm direct pressure if safe", "Wash the wound for several minutes before calling", "Remove any object stuck in the wound", "Ask the person to walk around"),
                Q(15, "When should a tourniquet be used for severe bleeding from an arm or leg?", 3,
                    "For every small cut", "Only after the person has been moved", "As a substitute for calling emergency services", "When available and the responder is trained to use it"),
                Q(16, "What is a recommended immediate action for a thermal burn?", 2,
                    "Apply ice directly to the burn", "Put butter on the burn", "Cool it with clean, cool running water", "Break any blisters"),
                Q(17, "What should you do while someone is having a seizure?", 1,
                    "Hold them down", "Protect them from nearby hazards and do not put anything in their mouth", "Put a spoon between their teeth", "Give them water"),
                Q(18, "What should a bystander do before using an AED?", 0,
                    "Turn it on and follow its prompts, making sure nobody touches the person when directed", "Guess where to place pads", "Wait for the device to deliver a shock automatically without prompts", "Place pads over clothing"),
                Q(19, "Why is scene safety still important during a medical emergency?", 2,
                    "It delays help", "Only the injured person can be harmed", "A responder who becomes injured may be unable to help", "It is needed only after CPR"),
                Q(20, "Can an online quiz replace hands-on CPR and first-aid training?", 3,
                    "Yes, a quiz alone qualifies everyone to provide advanced care", "Yes, if the score is high enough", "Yes, if the learner has watched a video", "No; this quiz supports learning but does not replace practical training or certification")
            };
        }

        private static List<QuestionItem> GetEmergencyCommunicationQuestions()
        {
            return new List<QuestionItem>
            {
                Q(1, "What should an initial emergency report communicate first?", 2,
                    "Who might be blamed", "Unconfirmed details from bystanders", "The exact location and the nature of the incident", "A long personal explanation"),
                Q(2, "What kind of language is clearest in an emergency message?", 0,
                    "Plain, concise language without unexplained jargon", "Slang and abbreviations only", "Long technical descriptions", "Speculation presented as fact"),
                Q(3, "Which location message is most useful to responders?", 1,
                    "It is somewhere in the building", "The building, entrance, floor or room, and a recognizable landmark", "It is near the usual meeting place", "A nearby town name only"),
                Q(4, "A dispatcher is asking questions while help is being sent. What should you do?", 3,
                    "Hang up to call a friend", "Guess when you do not know", "Talk over the dispatcher", "Stay on the line and answer as accurately as you can"),
                Q(5, "How should a team assign a person to call emergency services?", 2,
                    "Assume the newest bystander will do it", "Ask the whole group without checking", "Name one person, give the task, and confirm it was done", "Wait for the incident to end"),
                Q(6, "Which short incident message is most effective?", 1,
                    "A long narrative with opinions", "Who is calling, where the incident is, what happened, and what help is needed", "A list of unrelated nearby locations", "A message with no location until asked"),
                Q(7, "When using a two-way radio, what is a good general practice?", 0,
                    "Wait for the channel, speak clearly and briefly, then release it", "Hold the transmit button continuously", "Use several radios at once", "Speak before checking whether the channel is in use"),
                Q(8, "A responder repeats back a critical location or instruction. What should the sender do?", 3,
                    "Ignore the repeat-back", "Change the message without explaining", "Assume all details are correct", "Confirm it or correct any misunderstanding"),
                Q(9, "What is closed-loop communication?", 1,
                    "Sending a message without waiting for a response", "The receiver acknowledges the message and the sender confirms it was understood", "Repeating a rumor to several people", "Keeping all information within one person"),
                Q(10, "A radio channel is busy. What should you do?", 2,
                    "Repeat the same long message several times", "Use personal chat for urgent team instructions", "Keep the message brief and follow the channel protocol", "Transmit over another team's channel without permission"),
                Q(11, "Before sharing an emergency update publicly, what should you do?", 0,
                    "Verify it through an authorized, reliable source", "Share it because it sounds plausible", "Remove the time and source", "Add details that make it more dramatic"),
                Q(12, "You are not sure whether a report is confirmed. How should you communicate it?", 3,
                    "Describe it as confirmed", "Do not tell the response lead", "Change the details to be more definite", "Clearly label it as unconfirmed and pass it through the appropriate channel"),
                Q(13, "How should sensitive personal or medical information be shared?", 1,
                    "Post it on a public channel", "Share only what is necessary through an authorized channel with people who need it", "Send it to every group member", "Include it in a public announcement"),
                Q(14, "If the main communication network fails, what is the best preparation?", 2,
                    "Wait without a backup plan", "Use any frequency without coordination", "Use the agreed backup channel or communication procedure", "Assume all other teams know the situation"),
                Q(15, "A person has difficulty hearing or understanding your message. What should you do?", 0,
                    "Use an accessible method, speak clearly, and check understanding", "Shout more complex instructions", "Ask several people to speak at once", "Skip their questions"),
                Q(16, "During an organized response, which reporting path should team members generally use?", 3,
                    "Send every issue to every agency", "Use whichever manager is closest, without telling anyone", "Create a separate chain of command", "Follow the established supervisor and communication procedure"),
                Q(17, "What is one purpose of an incident command structure?", 1,
                    "Prevent teams from sharing information", "Coordinate roles, actions, and resources using a common structure", "Replace all emergency services", "Allow anyone to issue conflicting orders"),
                Q(18, "Which evacuation announcement is most useful?", 2,
                    "Leave soon, somehow", "There may be a problem somewhere", "State the affected area, the safe route, the destination, and the action people should take", "Repeat an unverified warning"),
                Q(19, "A new hazard appears after the first emergency call. What should happen?", 0,
                    "Update dispatch or the response lead promptly with the new hazard and location", "Keep the information to yourself", "Wait until the incident is over", "Post it publicly before notifying responders"),
                Q(20, "What should be included in a clear handoff to the next response team?", 3,
                    "Only the caller's name", "Personal opinions about the response", "Only information that is easy to remember", "Current situation, known hazards, actions taken, outstanding needs, and who is in charge"),
            };
        }

        private static QuestionItem Q(int questionId, string questionText, int correctOptionIndex,
            string optionA, string optionB, string optionC, string optionD)
        {
            string[] optionTexts = { optionA, optionB, optionC, optionD };
            var options = new List<OptionItem>();

            for (int index = 0; index < optionTexts.Length; index++)
            {
                options.Add(new OptionItem
                {
                    OptionID = ((questionId - 1) * optionTexts.Length) + index + 1,
                    OptionText = optionTexts[index],
                    OptionLabel = ((char)('A' + index)).ToString(),
                    IsCorrect = index == correctOptionIndex
                });
            }

            return new QuestionItem
            {
                QuestionID = questionId,
                QuestionText = questionText,
                QuestionType = "MultipleChoice",
                Points = 1,
                IsActive = true,
                Options = options
            };
        }
    }
}
