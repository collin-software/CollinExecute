using System;

namespace CollinExecute
{
    /// <summary>
    /// Represents errors that occur during shell command execution.
    /// </summary>
    public class ShellCommandException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellCommandException"/> class.
        /// </summary>
        public ShellCommandException() : base("An error occurred during shell command execution.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellCommandException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ShellCommandException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellCommandException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or <c>null</c> if no inner exception is specified.</param>
        public ShellCommandException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Represents errors that occur when a shell command execution times out.
    /// </summary>
    public class ShellCommandTimeoutException : ShellCommandException
    {
        /// <summary>
        /// Gets the timeout duration that was exceeded.
        /// </summary>
        public TimeSpan Timeout { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellCommandTimeoutException"/> class.
        /// </summary>
        /// <param name="timeout">The timeout duration that was exceeded.</param>
        public ShellCommandTimeoutException(TimeSpan timeout) 
            : base($"Shell command execution timed out after {timeout.TotalSeconds} seconds.")
        {
            Timeout = timeout;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellCommandTimeoutException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="timeout">The timeout duration that was exceeded.</param>
        public ShellCommandTimeoutException(string message, TimeSpan timeout) : base(message)
        {
            Timeout = timeout;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellCommandTimeoutException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="timeout">The timeout duration that was exceeded.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or <c>null</c> if no inner exception is specified.</param>
        public ShellCommandTimeoutException(string message, TimeSpan timeout, Exception innerException) 
            : base(message, innerException)
        {
            Timeout = timeout;
        }
    }
}
