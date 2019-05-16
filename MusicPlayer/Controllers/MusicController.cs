using CSCore;
using CSCore.Codecs;
using CSCore.Codecs.MP3;
using CSCore.DSP;
using CSCore.SoundOut;
using CSCore.Streams;
using CSCore.Streams.Effects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinformsVisualization.Visualization;

namespace MusicPlayer.Controllers
{
    public class MusicController
    {
        private static LineSpectrum _lineSpectrum;
        private static PitchShifter _pitchShifter;

        private static ISoundOut _soundOut;
        private static IWaveSource _waveSource;
        public static PlaybackState playbackState;

        public static LineSpectrum LineSpectrum { get => _lineSpectrum; set => _lineSpectrum = value; }

        public static void Play(string file)
        {

            _soundOut = new WasapiOut();

            if (playbackState == PlaybackState.Playing) Stop();

            ISampleSource source = CodecFactory.Instance.GetCodec(file).ToSampleSource()
                .AppendSource(x => new PitchShifter(x), out _pitchShifter);
            SetupSampleSource(source);

            _soundOut.Initialize(_waveSource);
            _soundOut.Play();
            playbackState = PlaybackState.Playing;

        }

        public static void Play()
        {
            _soundOut.Play();
            playbackState = PlaybackState.Playing;
        }

        public static void Pause()
        {
            _soundOut.Pause();
            playbackState = PlaybackState.Paused;
        }

        public static void Stop()
        {
            if (_soundOut != null)
            {
                _soundOut.Stop();
            }

            if (_waveSource != null)
            {
                _waveSource.Dispose();
                _waveSource = null;
            }
            playbackState = PlaybackState.Stopped;
        }

        public static string CurrentTime()
        {
            if (_waveSource != null)
                return _waveSource.GetPosition().ToString(@"mm\:ss");
            else
                return null;
        }

        public static void Dispose()
        {
            if (_soundOut != null)
            {
                _soundOut.Dispose();
                _soundOut = null;
            }
            if (_waveSource != null)
            {
                _waveSource.Dispose();
                _waveSource = null;
            }
        }

        private static void SetupSampleSource(ISampleSource aSampleSource)
        {
            const FftSize fftSize = FftSize.Fft4096;
            //create a spectrum provider which provides fft data based on some input
            var spectrumProvider = new BasicSpectrumProvider(aSampleSource.WaveFormat.Channels,
                aSampleSource.WaveFormat.SampleRate, fftSize);

            //linespectrum and voiceprint3dspectrum used for rendering some fft data
            //in oder to get some fft data, set the previously created spectrumprovider 
            _lineSpectrum = new LineSpectrum(fftSize)
            {
                SpectrumProvider = spectrumProvider,
                UseAverage = true,
                BarCount = 50,
                BarSpacing = 2,
                IsXLogScale = true,
                ScalingStrategy = ScalingStrategy.Sqrt
            };
            //the SingleBlockNotificationStream is used to intercept the played samples
            var notificationSource = new SingleBlockNotificationStream(aSampleSource);
            //pass the intercepted samples as input data to the spectrumprovider (which will calculate a fft based on them)
            notificationSource.SingleBlockRead += (s, a) => spectrumProvider.Add(a.Left, a.Right);

            _waveSource = notificationSource.ToWaveSource(16);
        }

    }
}
