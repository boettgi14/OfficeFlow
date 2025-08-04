using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeFlow
{
    /// <summary>
    /// Represents a time entry with a start and end point, a unique identifier, and an optional pause duration.
    /// </summary>
    /// <remarks>This class is used to model a time period with a defined start and end, along with an
    /// optional pause duration. It provides a calculated <see cref="Duration"/> property to determine the total elapsed
    /// time between the start and end points. Ensure that the <see cref="Pause"/> property is set to a non-negative
    /// value to avoid exceptions.</remarks>
    internal class Time
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the start date and time of the event or operation.
        /// </summary>
        public DateTime Start { get; set; }
        /// <summary>
        /// Gets or sets the end date and time of the event or time period.
        /// </summary>
        public DateTime End { get; set; }
        /// <summary>
        /// Gets or sets the total duration of the operation or event.
        /// </summary>
        public TimeSpan TotalDuration { get; set; }
        /// <summary>
        /// Gets or sets the duration of the pause between operations.
        /// </summary>
        /// <remarks>Use this property to introduce a delay between operations, such as retries or
        /// periodic tasks.  Ensure the value is non-negative; setting a negative value will result in an <see
        /// cref="ArgumentOutOfRangeException"/>.</remarks>
        public TimeSpan PauseDuration { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Time"/> class with the specified parameters.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="totalDuration"></param>
        /// <param name="pauseDuration"></param>
        public Time(int id, DateTime start, DateTime end, TimeSpan totalDuration, TimeSpan pauseDuration)
        {
            this.Id = id;
            this.Start = start;
            this.End = end;
            this.TotalDuration = totalDuration;
            this.PauseDuration = pauseDuration;
        }
    }
}
