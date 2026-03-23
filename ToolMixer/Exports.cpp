#include "ToolMixer.cpp"

extern "C" __declspec(dllexport) bool ActivateToolMixer()
{
    try
    {
        if(!Object::ReferenceEquals(Mixer,nullptr))
        {
            return Mixer->Activate();
        }
        return false;
    }
    catch( Exception^ ) { return false; }
}

extern "C" __declspec(dllexport) bool CreateToolMixer()
{
    try
    {
        if(Object::ReferenceEquals(Mixer,nullptr))
        {
            Mixer = gcnew ToolMixer(); return true;
        }
        return false;
    }
    catch( Exception^ ) { return false; }
}

extern "C" __declspec(dllexport) bool DeactivateToolMixer()
{
    try
    {
        if(!Object::ReferenceEquals(Mixer,nullptr))
        {
            return Mixer->Deactivate();
        }
        return false;
    }
    catch( Exception^ ) { return false; }
}

extern "C" __declspec(dllexport) bool DeleteToolMixer()
{
    try
    {
        if(!Object::ReferenceEquals(Mixer,nullptr))
        {
            delete Mixer; return true;
        }
        return false;
    }
    catch( Exception^ ) { return false; }
}