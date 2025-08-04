using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeFlow
{
    /// <summary>
    /// Provides functionality to track elapsed time for operations, with support for pausing and resuming.
    /// </summary>
    /// <remarks>The <see cref="TimeTracker"/> class is designed to measure the total elapsed time of an
    /// operation,  excluding any periods during which the operation is paused. It supports starting, pausing, resuming,
    /// and stopping the time tracking process. This class is useful for scenarios where precise timing  is required,
    /// such as performance monitoring or task duration tracking.</remarks>
    internal class TimeTracker
    {
        /// <summary>
        /// A stopwatch used to measure elapsed time for operations.
        /// </summary>
        /// <remarks>This field is used internally to track the duration of specific operations.  It is
        /// not exposed publicly and should only be accessed within the containing class.</remarks>
        private Stopwatch Stopwatch;
        /// <summary>
        /// Represents the start time of an operation or event.
        /// </summary>
        /// <remarks>This field stores the initial timestamp when the operation or event begins.  It is
        /// intended for internal use and should not be accessed directly outside the class.</remarks>
        private DateTime StartTime;
        /// <summary>
        /// Represents the duration for which an operation or process has been paused.
        /// </summary>
        /// <remarks>This field is used to track the total time spent in a paused state.  It is intended
        /// for internal use and should not be accessed directly by external code.</remarks>
        private TimeSpan PauseDuration;
        /// <summary>
        /// Represents the timestamp when a pause operation started.
        /// </summary>
        /// <remarks>This field is used to track the start time of a pause operation.  It is intended for
        /// internal use and should not be accessed directly by external code.</remarks>
        private DateTime PauseStartTime;
        /// <summary>
        /// Indicates whether the current operation is paused.
        /// </summary>
        private bool IsPaused;
        /// <summary>
        /// Represents the currently logged-in user.
        /// </summary>
        /// <remarks>This field holds the user information for the active session.  It is intended for
        /// internal use and should not be accessed directly outside of the class.</remarks>
        private User CurrentUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeTracker"/> class,  which provides functionality to track
        /// elapsed time with support for pausing and resuming.
        /// </summary>
        public TimeTracker(User user)
        {
            Stopwatch = new Stopwatch();
            PauseDuration = TimeSpan.Zero;
            IsPaused = false;
            CurrentUser = user;
        }

        /// <summary>
        /// Starts the stopwatch if it is not already running and is not paused.
        /// </summary>
        /// <remarks>This method initializes the stopwatch by setting the start time to the current date
        /// and time  and begins tracking elapsed time. If the stopwatch is already running or paused, the method  does
        /// nothing and returns <see langword="0"/>.</remarks>
        /// <returns><see langword="1"/> if the stopwatch was successfully started; otherwise, <see langword="0"/>.</returns>
        public int Start()
        {
            if (!Stopwatch.IsRunning && !IsPaused)
            {
                // Starten der Stoppuhr
                StartTime = DateTime.Now;
                Stopwatch.Start();

                return 1;
            }
            return 0;
        }

        /// <summary>
        /// Pauses the stopwatch if it is currently running and not already paused.
        /// </summary>
        /// <remarks>This method stops the stopwatch and marks it as paused. If the stopwatch is not
        /// running  or is already paused, the method does nothing and returns <see langword="0"/>.</remarks>
        /// <returns><see langword="1"/> if the stopwatch was successfully paused; otherwise, <see langword="0"/>.</returns>
        public int Pause()
        {
            if (Stopwatch.IsRunning && !IsPaused)
            {
                // Pausieren der Stoppuhr und Speichern der Startzeit der Pause
                PauseStartTime = DateTime.Now;
                Stopwatch.Stop();
                IsPaused = true;

                return 1;
            }
            return 0;
        }

        /// <summary>
        /// Resumes the stopwatch if it is currently paused.
        /// </summary>
        /// <remarks>This method resumes the stopwatch only if it is paused and not already running.  The
        /// total pause duration is updated to include the time elapsed since the stopwatch was paused.</remarks>
        /// <returns><see langword="1"/> if the stopwatch was successfully resumed; otherwise, <see langword="0"/>.</returns>
        public int Resume()
        {
            if (!Stopwatch.IsRunning && IsPaused)
            {
                // Fortsetzen der Stoppuhr und Berechnung der Pause
                TimeSpan pauseDuration = DateTime.Now - PauseStartTime;
                PauseDuration += pauseDuration;
                Stopwatch.Start();
                IsPaused = false;

                return 1;
            }
            return 0;
        }

        /// <summary>
        /// Stops the stopwatch and records the elapsed time, including any paused duration.
        /// </summary>
        /// <remarks>If the stopwatch is running or paused, this method calculates the total elapsed time,
        /// including any paused duration, and saves the timing information to the database. If the stopwatch is neither
        /// running nor paused, the method does nothing and returns 0.</remarks>
        /// <returns>Returns <see langword="1"/> if the stopwatch was successfully stopped and the time was recorded; otherwise,
        /// <see langword="0"/>.</returns>
        public int Stop()
        {
            if (Stopwatch.IsRunning || IsPaused)
            {
                // Setzen der Endzeit
                DateTime endTime = DateTime.Now;

                if (IsPaused)
                {
                    // Berechnung der Pause falls die Stoppuhr pausiert war
                    TimeSpan pauseDuration = endTime - PauseStartTime;
                    PauseDuration += pauseDuration;
                }
                else
                {
                    // Stoppen der Stoppuhr
                    Stopwatch.Stop();
                }

                // Berechnung der Gesamtzeit
                TimeSpan totalDuration = endTime - StartTime - PauseDuration;

                // Abspeichern der Zeit in der Datenbank
                TimeDatabaseHelper.AddTime(CurrentUser.Id, StartTime, endTime, totalDuration.Minutes, PauseDuration.Minutes);

                return 1;
            }
            return 0;
        }
    }
}
