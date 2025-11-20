import React from 'react';
import { StrictMode } from 'react';
import ToolGUI from './ToolGUI/ToolGUI';
import { createRoot } from 'react-dom/client';

class StartUp
{
    static Main()
    {
        try
        {
            const container = document.getElementById('root');
            if(container)
            {
                const root = createRoot(container);
                root.render(<ToolGUI />);
            }
        }
        catch (error)
        {
            console.error('[StartUp] Failed to initialize application:',error);
        }
    }
}

StartUp.Main();