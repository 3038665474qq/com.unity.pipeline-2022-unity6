using Unity.Pipeline.Commands;

namespace Unity.Pipeline.Editor.Commands.Observability
{
    /// <summary>
    /// Records Pipeline analytics events (local recording only on Unity 2022) and lists the most recent ones, so a client
    /// can check what a command it ran was reported as. Nothing is recorded until a client asks.
    /// </summary>
    static class AnalyticsEventsCommand
    {
        const string Group = "diagnostics";

        [CliGroupCommand(Group,
            "Start recording Pipeline analytics events (local recording only on Unity 2022), for diagnostics recent_analytics to list. Recording lasts until the next domain reload.",
            Tags = new[] { "observability/analytics" },
            Effect = CommandEffect.Idempotent)]
        internal static ExitCode RecordAnalytics(CommandContext cmd)
        {
            PipelineAnalytics.IsRecording = true;
#if UNITY_6000_0_OR_NEWER
            cmd.Log.Info("Recording analytics events.");
#else
            cmd.Log.Info("Recording analytics events locally. Sending analytics is unsupported on Unity 2022.");
#endif
            return ExitCode.Success;
        }

        // Main thread: a command's event is recorded by a main-thread post queued before its reply,
        // so a read that queues behind it always sees it.
        [CliGroupCommand(Group,
            "List the last Pipeline analytics events recorded, newest last: each one's name, time and payload. Empty until diagnostics record_analytics starts recording.",
            Tags = new[] { "observability/analytics" },
            Effect = CommandEffect.ReadOnly)]
        internal static ExitCode RecentAnalytics(CommandContext cmd)
        {
            foreach (var sent in PipelineAnalytics.RecentEvents)
                cmd.Result.WriteRecord(sent);
            return ExitCode.Success;
        }
    }
}
