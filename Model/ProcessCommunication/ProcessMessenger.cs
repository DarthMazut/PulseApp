using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ProcessCommunication
{
    /// <summary>
    /// Provides an easy way for retrieving output from external process.
    /// </summary>
    public class ProcessMessenger
    {
        private readonly string _processPath;
        private readonly string? _workingDirectory;
        private readonly Dictionary<Process, ProcessConnection> _processConnections = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessMessenger"/> class.
        /// </summary>
        /// <param name="processPath">Path to the target process, which output is to be retrieved.</param>
        public ProcessMessenger(string processPath) : this(processPath, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessMessenger"/> class.
        /// </summary>
        /// <param name="processPath">Path to the target process, which output is to be retrieved.</param>
        /// <param name="workingDirectory">Working directory of target process.</param>
        public ProcessMessenger(string processPath, string? workingDirectory)
        {
            _processPath = processPath;
            _workingDirectory = workingDirectory;
        }

        /// <summary>
        /// Executes specified process with given arguments (command). 
        /// Asynchronously returns output from that process.
        /// </summary>
        /// <param name="command">Parameters that target process will be launched with.</param>
        public Task<ProcessCommandOutput> SendCommandAndWaitForResponseAsync(string command)
            => SendCommandAndWaitForResponseAsync(command, null, default);

        /// <summary>
        /// Executes specified process with given arguments (command). 
        /// Asynchronously returns output from that process.
        /// </summary>
        /// <param name="command">Parameters that target process will be launched with.</param>
        /// <param name="progress">Reports progress of ongoing process in real time.</param>
        public Task<ProcessCommandOutput> SendCommandAndWaitForResponseAsync(string command, IProgress<ProcessCommandPartialOutput>? progress)
            => SendCommandAndWaitForResponseAsync(command, progress, default);

        /// <summary>
        /// Executes specified process with given arguments (command). 
        /// Asynchronously returns output from that process.
        /// </summary>
        /// <param name="command">Parameters that target process will be launched with.</param>
        /// <param name="progress">Reports progress of ongoing process in real time.</param>
        /// <param name="cancellationToken">Provides possibility for cancelling started process.</param>
        public Task<ProcessCommandOutput> SendCommandAndWaitForResponseAsync(string? command, IProgress<ProcessCommandPartialOutput>? progress, CancellationToken cancellationToken)
        {
            Process process = SetupProcess(command);
            TaskCompletionSource<ProcessCommandOutput> tcs = new();
            CancellationTokenRegistration registration = cancellationToken.Register(() =>
            {
                _processConnections[process].MarkAborted();
                process.Kill();
            });
            _processConnections.Add(process, new ProcessConnection(tcs, progress, registration));
            StartProcess(process);

            return tcs.Task;
        }

        private Process SetupProcess(string? command)
        {
            Process process = new();

            process.StartInfo = CreateProcesStartInfo(_processPath, command);
            process.EnableRaisingEvents = true;
            process.OutputDataReceived += OnProcessOutputReceived;
            process.ErrorDataReceived += OnProcessErrorReceived;
            process.Exited += OnProcessFinished;

            return process;
        }

        private void StartProcess(Process process)
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        private ProcessStartInfo CreateProcesStartInfo(string processPath, string? command)
        {
            return new ProcessStartInfo(processPath)
            {
                Arguments = command,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = _workingDirectory
            };
        }

        private void OnProcessOutputReceived(object sender, DataReceivedEventArgs e)
        {
            HandleOutputReceived(sender, e, false);
        }

        private void OnProcessErrorReceived(object sender, DataReceivedEventArgs e)
        {
            HandleOutputReceived(sender, e, true);
        }

        private void OnProcessFinished(object? sender, EventArgs e)
        {
            if (sender is Process process)
            {
                ProcessConnection connection = _processConnections[process];
                CleanUpProcess(process);
                if (connection.IsAborted)
                {
                    connection.TaskCompletionSource.TrySetException(new OperationCanceledException());
                }
                else
                {
                    connection.TaskCompletionSource.TrySetResult(connection.OutputBuilder.BuildOutput());
                }
            }
        }

        private void HandleOutputReceived(object sender, DataReceivedEventArgs e, bool isError)
        {
            if (e.Data is not null)
            {
                if (sender is Process process)
                {
                    ProcessConnection connection = _processConnections[process];
                    ProcessCommandPartialOutput partialOutput = new ProcessCommandPartialOutput(e.Data, isError);
                    connection.OutputBuilder.AddOutput(partialOutput);
                    connection.Progress?.Report(partialOutput);
                }
            }
        }

        private void CleanUpProcess(Process process)
        {
            CancellationTokenRegistration registration = _processConnections[process].TokenRegistration;
            registration.Unregister();
            registration.Dispose();
            _processConnections.Remove(process);
            process.Dispose();
        }

        private class ProcessConnection
        {
            public ProcessConnection(
                TaskCompletionSource<ProcessCommandOutput> tcs,
                IProgress<ProcessCommandPartialOutput>? progress,
                CancellationTokenRegistration tokenRegistration = default)
            {
                TaskCompletionSource = tcs;
                Progress = progress;
                TokenRegistration = tokenRegistration;
            }

            public TaskCompletionSource<ProcessCommandOutput> TaskCompletionSource { get; }

            public IProgress<ProcessCommandPartialOutput>? Progress { get; }

            public CancellationTokenRegistration TokenRegistration { get; }

            public ProcessCommandOutputBuilder OutputBuilder { get; } = new();

            public bool IsAborted { get; private set; }

            public void MarkAborted()
            {
                IsAborted = true;
            }
        }
    }
}
