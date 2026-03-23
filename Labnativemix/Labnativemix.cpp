#include <iostream>
#include <Windows.h>
#include "ToolMixer.h"
using namespace std;

int main()
{
    HMODULE MixerModule = LoadLibraryW(L"KusDepot.ToolMixer.dll");

    if(MixerModule == NULL) { cerr << "KusDepot.ToolMixer.dll load failed."; return -1; }

    CreateToolMixer Create = (CreateToolMixer)GetProcAddress(MixerModule,"CreateToolMixer");

    DeleteToolMixer Delete = (DeleteToolMixer)GetProcAddress(MixerModule,"DeleteToolMixer");

    ActivateToolMixer Activate = (ActivateToolMixer)GetProcAddress(MixerModule,"ActivateToolMixer");

    DeactivateToolMixer Deactivate = (DeactivateToolMixer)GetProcAddress(MixerModule,"DeactivateToolMixer");

    cout << Create(); cout << Activate(); cout << Deactivate(); cout << Delete(); return 0;
}