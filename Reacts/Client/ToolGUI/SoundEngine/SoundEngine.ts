export default class SoundEngine
{
    private context: AudioContext | null = null;
    private masterGain: GainNode | null = null;
    private muted: boolean = false;

    private static readonly MASTER_VOLUME = 0.25;

    private ensureContext(): AudioContext | null
    {
        try
        {
            if (!this.context)
            {
                this.context = new AudioContext();
                this.masterGain = this.context.createGain();
                this.masterGain.gain.value = this.muted ? 0 : SoundEngine.MASTER_VOLUME;
                this.masterGain.connect(this.context.destination);
            }

            if (this.context.state === "suspended")
            {
                this.context.resume();
            }

            return this.context;
        }
        catch (error)
        {
            console.error('[SoundEngine] Error in ensureContext:', error);
            return null;
        }
    }

    toggleMute(): boolean
    {
        try
        {
            this.muted = !this.muted;
            if (this.masterGain)
            {
                this.masterGain.gain.value = this.muted ? 0 : SoundEngine.MASTER_VOLUME;
            }
            return this.muted;
        }
        catch (error)
        {
            console.error('[SoundEngine] Error in toggleMute:', error);
            return this.muted;
        }
    }

    playCreation(star: boolean | null | undefined, circle: boolean | null | undefined): void
    {
        try
        {
            const ctx = this.ensureContext();
            if (!ctx || !this.masterGain) { return; }

            const now = ctx.currentTime;

            const baseFreq = star ? 440 : circle ? 220 : 330;
            const detuneCents = (Math.random() - 0.5) * 6;

            const osc = ctx.createOscillator();
            osc.type = "sine";
            osc.frequency.value = baseFreq;
            osc.detune.value = detuneCents;

            const gain = ctx.createGain();
            gain.gain.setValueAtTime(0, now);
            gain.gain.linearRampToValueAtTime(0.6, now + 0.01);
            gain.gain.exponentialRampToValueAtTime(0.001, now + 0.3);

            osc.connect(gain);
            gain.connect(this.masterGain);

            osc.start(now);
            osc.stop(now + 0.35);
        }
        catch (error)
        {
            console.error('[SoundEngine] Error in playCreation:', error);
        }
    }

    playMessage(): void
    {
        try
        {
            const ctx = this.ensureContext();
            if (!ctx || !this.masterGain) { return; }

            const now = ctx.currentTime;
            const frequencies = [523, 784];

            for (const freq of frequencies)
            {
                const osc = ctx.createOscillator();
                osc.type = "sine";
                osc.frequency.value = freq;

                const gain = ctx.createGain();
                gain.gain.setValueAtTime(0, now);
                gain.gain.linearRampToValueAtTime(0.3, now + 0.005);
                gain.gain.exponentialRampToValueAtTime(0.001, now + 0.15);

                osc.connect(gain);
                gain.connect(this.masterGain);

                osc.start(now);
                osc.stop(now + 0.2);
            }
        }
        catch (error)
        {
            console.error('[SoundEngine] Error in playMessage:', error);
        }
    }

    playPurge(): void
    {
        try
        {
            const ctx = this.ensureContext();
            if (!ctx || !this.masterGain) { return; }

            const now = ctx.currentTime;

            const osc = ctx.createOscillator();
            osc.type = "sine";
            osc.frequency.setValueAtTime(60, now);
            osc.frequency.exponentialRampToValueAtTime(30, now + 0.5);

            const oscGain = ctx.createGain();
            oscGain.gain.setValueAtTime(0.7, now);
            oscGain.gain.exponentialRampToValueAtTime(0.001, now + 0.6);

            osc.connect(oscGain);
            oscGain.connect(this.masterGain);

            osc.start(now);
            osc.stop(now + 0.65);

            const bufferSize = ctx.sampleRate * 0.6;
            const noiseBuffer = ctx.createBuffer(1, bufferSize, ctx.sampleRate);
            const data = noiseBuffer.getChannelData(0);
            for (let i = 0; i < bufferSize; i++)
            {
                data[i] = (Math.random() * 2 - 1) * 0.3;
            }

            const noise = ctx.createBufferSource();
            noise.buffer = noiseBuffer;

            const filter = ctx.createBiquadFilter();
            filter.type = "lowpass";
            filter.frequency.setValueAtTime(120, now);
            filter.frequency.exponentialRampToValueAtTime(40, now + 0.5);

            const noiseGain = ctx.createGain();
            noiseGain.gain.setValueAtTime(0.4, now);
            noiseGain.gain.exponentialRampToValueAtTime(0.001, now + 0.55);

            noise.connect(filter);
            filter.connect(noiseGain);
            noiseGain.connect(this.masterGain);

            noise.start(now);
            noise.stop(now + 0.6);
        }
        catch (error)
        {
            console.error('[SoundEngine] Error in playPurge:', error);
        }
    }

    playRemoval(): void
    {
        try
        {
            const ctx = this.ensureContext();
            if (!ctx || !this.masterGain) { return; }

            const now = ctx.currentTime;
            const detuneCents = (Math.random() - 0.5) * 6;

            const osc = ctx.createOscillator();
            osc.type = "sine";
            osc.frequency.setValueAtTime(400, now);
            osc.frequency.exponentialRampToValueAtTime(200, now + 0.25);
            osc.detune.value = detuneCents;

            const gain = ctx.createGain();
            gain.gain.setValueAtTime(0, now);
            gain.gain.linearRampToValueAtTime(0.35, now + 0.008);
            gain.gain.exponentialRampToValueAtTime(0.001, now + 0.25);

            osc.connect(gain);
            gain.connect(this.masterGain);

            osc.start(now);
            osc.stop(now + 0.3);
        }
        catch (error)
        {
            console.error('[SoundEngine] Error in playRemoval:', error);
        }
    }

    playRandomize(): void
    {
        try
        {
            const ctx = this.ensureContext();
            if (!ctx || !this.masterGain) { return; }

            const now = ctx.currentTime;
            const frequencies = [330, 415];
            const offsets = [0, 0.015];

            for (let i = 0; i < frequencies.length; i++)
            {
                const osc = ctx.createOscillator();
                osc.type = "sine";
                osc.frequency.value = frequencies[i];

                const gain = ctx.createGain();
                const start = now + offsets[i];
                gain.gain.setValueAtTime(0, start);
                gain.gain.linearRampToValueAtTime(0.25, start + 0.005);
                gain.gain.exponentialRampToValueAtTime(0.001, start + 0.12);

                osc.connect(gain);
                gain.connect(this.masterGain);

                osc.start(start);
                osc.stop(start + 0.15);
            }
        }
        catch (error)
        {
            console.error('[SoundEngine] Error in playRandomize:', error);
        }
    }
}
