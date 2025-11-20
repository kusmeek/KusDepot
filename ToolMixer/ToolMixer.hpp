using namespace System;
using namespace KusDepot;

public ref class ToolMixer
{
    private:
        Tool^ tool;

    public:
        ToolMixer();
        ~ToolMixer();
        !ToolMixer();
        bool Activate();
        bool Deactivate();
};