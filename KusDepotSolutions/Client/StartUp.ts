import "@fontsource/inter/latin-300.css";
import "@fontsource/inter/latin-400.css";
import "@fontsource/inter/latin-500.css";
import "@fontsource/inter/latin-600.css";
import "./site.css";

class StartUp
{
    static Main()
    {
        try
        {
            StartUp.initNav();
            StartUp.initAmbientOrbs();
            console.log('[KusDepotSolutions] Loaded');
        }
        catch (error)
        {
            console.error('[StartUp] Failed to initialize application:',error);
        }
    }

    private static initNav()
    {
        const toggle = document.querySelector<HTMLButtonElement>('.topbar-toggle');
        const links  = document.querySelector<HTMLDivElement>('.topbar-links');

        if (!toggle || !links) return;

        toggle.addEventListener('click', () =>
        {
            const open = toggle.classList.toggle('open');
            links.classList.toggle('open', open);
            toggle.setAttribute('aria-expanded', String(open));
        });

        links.addEventListener('click', (e) =>
        {
            if ((e.target as HTMLElement).tagName === 'A')
            {
                toggle.classList.remove('open');
                links.classList.remove('open');
                toggle.setAttribute('aria-expanded', 'false');
            }
        });
    }

    private static initAmbientOrbs()
    {
        if (matchMedia('(pointer: coarse)').matches) return;
        if (matchMedia('(prefers-reduced-motion: reduce)').matches) return;

        const count = 4;

        for (let i = 0; i < count; i++)
        {
            const orb = document.createElement('div');
            orb.className = 'ambient-orb';

            const size = 28 + Math.random() * 24;
            const color = StartUp.createAmbientColor();
            const blur = 40 + Math.random() * 30;

            orb.style.width  = size + 'rem';
            orb.style.height = size + 'rem';
            orb.style.background = `radial-gradient(circle, ${color} 0%, transparent 68%)`;
            orb.style.filter = `blur(${blur}px) brightness(1.8)`;

            document.body.appendChild(orb);
            StartUp.driftOrb(orb);
        }
    }

    private static createAmbientColor()
    {
        const variance = 0.15;
        const brightness = 0.16 + Math.random() * 0.12;
        const primary = Math.floor(Math.random() * 3);

        const channels = [0, 1, 2].map(index =>
        {
            if (index === primary) return Math.round(255 * brightness);

            const offset = Math.random() * variance;
            return Math.round(255 * brightness * offset);
        });

        const alpha = 0.05 + Math.random() * 0.04;

        return `rgba(${channels[0]}, ${channels[1]}, ${channels[2]}, ${alpha.toFixed(3)})`;
    }

    private static driftOrb(orb: HTMLElement)
    {
        const vw = window.innerWidth;
        const vh = window.innerHeight;

        const startX = Math.random() * vw;
        const startY = Math.random() * vh * 2;

        const nextX = () => Math.random() * vw;
        const nextY = () => Math.random() * vh * 2;
        const nextDur = () => 18000 + Math.random() * 22000;
        const nextOpacity = () => 0.58 + Math.random() * 0.32;

        let curX = startX;
        let curY = startY;

        orb.style.transform = `translate(${curX}px, ${curY}px)`;

        const fadeIn = orb.animate(
            [{ opacity: 0 }, { opacity: nextOpacity() }],
            { duration: 3000, fill: 'forwards', easing: 'ease-in-out' }
        );

        fadeIn.onfinish = () => move();

        function move()
        {
            const tx = nextX();
            const ty = nextY();
            const op = nextOpacity();
            const dur = nextDur();

            const anim = orb.animate(
            [
                { transform: `translate(${curX}px, ${curY}px)`, opacity: getComputedStyle(orb).opacity },
                { transform: `translate(${tx}px, ${ty}px)`, opacity: op }
            ],
            { duration: dur, fill: 'forwards', easing: 'ease-in-out' });

            anim.onfinish = () =>
            {
                curX = tx;
                curY = ty;
                orb.style.transform = `translate(${curX}px, ${curY}px)`;
                move();
            };
        }
    }
}

StartUp.Main();
