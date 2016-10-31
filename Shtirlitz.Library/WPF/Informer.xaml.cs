using Shtirlitz.Reporter;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System;
using System.Threading;

namespace Shtirlitz.WPF
{

    public enum InformerState
    {
        Unknown,
        Progress,
        Generated,
        Canceled,
        Failed
    }

    class InformerCommand : ICommand
    {
        private Action<object> _action;
        private Predicate<object> _canExecute;

        public InformerCommand(Action<object> action, Predicate<object> canExecute = null)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public void DoCanExecuteChanged(object sender)
        {
            CanExecuteChanged?.Invoke(sender ?? this, new EventArgs());
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute.Invoke(parameter);
        }

        public void Execute(object parameter)
        {
            _action.Invoke(parameter);
        }
    }


    /// <summary>
    /// Логика взаимодействия для Informer.xaml
    /// </summary>
    public partial class Informer : UserControl
    {
        #region DependencyProperties

        #region Shtirlit
        public static readonly DependencyProperty ShtirlitzProperty =
            DependencyProperty.Register("Shtirlitz", typeof(IShtirlitz), typeof(Informer),
            new PropertyMetadata(null, ShtirlitzPropertyChanged));

        public IShtirlitz Shtirlitz
        {
            get { return (IShtirlitz)GetValue(ShtirlitzProperty); }
            set { SetValue(ShtirlitzProperty, value); }
        }

        private static void ShtirlitzPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Informer)d).ShtirlitzPropertyChanged((IShtirlitz)e.OldValue, (IShtirlitz)e.NewValue);
        }

        private void ShtirlitzPropertyChanged(IShtirlitz oldShtirlitz, IShtirlitz newShtirlitz)
        {
            if (oldShtirlitz != null)
            {
                oldShtirlitz.GenerationProgress -= OnGenerationProgress;
                oldShtirlitz.ReportCanceled -= OnReportCanceled;
                oldShtirlitz.ReportGenerated -= OnReportGenerated;
            }
            if (newShtirlitz != null)
            {
                newShtirlitz.GenerationProgress += OnGenerationProgress;
                newShtirlitz.ReportCanceled += OnReportCanceled;
                newShtirlitz.ReportGenerated += OnReportGenerated;
            }
            State = InformerState.Unknown;
            // TODO: duplicate event
            // _cancelCommand.DoCanExecuteChanged(this);
        }

        #endregion //ReporterProperty

        #region CancellationTokenSource
        public static readonly DependencyProperty CancellationTokenSourceProperty =
            DependencyProperty.Register("CancellationTokenSource", typeof(CancellationTokenSource), typeof(Informer),
            new PropertyMetadata(null, CancellationTokenSourcePropertyChanged));

        public CancellationTokenSource CancellationTokenSource
        {
            get { return (CancellationTokenSource)GetValue(CancellationTokenSourceProperty); }
            set { SetValue(CancellationTokenSourceProperty, value); }
        }

        private static void CancellationTokenSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Informer)d)._cancelCommand.DoCanExecuteChanged(d);
        }
        #endregion // CancellationTokenSource

        #region Stage
        private static readonly DependencyPropertyKey StagePropertyKey =
            DependencyProperty.RegisterReadOnly("Stage", typeof(string), typeof(Informer),
                new PropertyMetadata(""));

        public static readonly DependencyProperty StageProperty =
            StagePropertyKey.DependencyProperty;

        public string Stage
        {
            get { return (string)GetValue(StageProperty); }
            protected set { SetValue(StagePropertyKey, value); }
        }
        #endregion // Stage

        #region Progress
        private static readonly DependencyPropertyKey ProgressPropertyKey =
            DependencyProperty.RegisterReadOnly("Progress", typeof(double), typeof(Informer),
                new PropertyMetadata(double.NaN));

        public static readonly DependencyProperty ProgressProperty =
            ProgressPropertyKey.DependencyProperty;

        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            protected set { SetValue(ProgressPropertyKey, value); }
        }
        #endregion // Progress

        #region State
        private static readonly DependencyPropertyKey StatePropertyKey =
            DependencyProperty.RegisterReadOnly("State", typeof(InformerState), typeof(Informer),
                new PropertyMetadata(InformerState.Unknown, StatePropertyChanged));

        public static readonly DependencyProperty StateProperty =
            StatePropertyKey.DependencyProperty;

        public InformerState State
        {
            get { return (InformerState)GetValue(StateProperty); }
            protected set { SetValue(StatePropertyKey, value); }
        }

        private static void StatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Informer)d)._cancelCommand.DoCanExecuteChanged(d);
        }
        #endregion // State

        #endregion // DependencyProperties

        #region Commands

        #region InformerCommand
        private readonly InformerCommand _cancelCommand;
        public ICommand CancelCommand
        {
            get { return _cancelCommand; }
        }

        private void DoCancelCommand(object parameter)
        {
            if (IsCanCancel())
            {
                CancellationTokenSource.Cancel();                
            }
        }
        #endregion // InformerCommand

        #endregion // Commands

        public Informer()
        {
            InitializeComponent();
            _cancelCommand = new InformerCommand(DoCancelCommand, IsCanCancel);
        }

        #region Shtirlitz EventHendlers

        private void OnGenerationProgress(object sender, GenerationProgressEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => {
                Stage = e.StageName;
                Progress = e.Progress;
                State = InformerState.Progress;
            }));

        }

        private void OnReportCanceled(object sender, ReportCanceledEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => {
                State = e.HasFailed ? InformerState.Failed : InformerState.Canceled;
            }));
        }

        private void OnReportGenerated(object sender, ReportGeneratedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => {
                State = InformerState.Generated;
            }));
        }

        #endregion Shtirlitz EventHendlers


        private bool IsCanCancel(object parameter = null)
        {
            return Shtirlitz != null && CancellationTokenSource != null && State == InformerState.Progress;
        }

    }
}
