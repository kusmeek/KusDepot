#using <Microsoft.Extensions.Logging.Abstractions.dll>
#using <Microsoft.Extensions.Configuration.Abstractions.dll>
#using <Microsoft.Extensions.DependencyInjection.Abstractions.dll>
#include "Globals.cpp"

ToolMixer::!ToolMixer() { }

ToolMixer::ToolMixer() { this->tool = gcnew Tool(); }

ToolMixer::~ToolMixer() { delete this->tool; this->!ToolMixer(); }

bool ToolMixer::Activate() { this->tool->Activate(System::Threading::CancellationToken::None , gcnew HostKey(Guid::NewGuid().ToByteArray(),Guid::NewGuid())); return this->tool->GetLifeCycleState() == LifeCycleState::Active; }

bool ToolMixer::Deactivate() { this->tool->Deactivate(System::Threading::CancellationToken::None , gcnew HostKey(Guid::NewGuid().ToByteArray(),Guid::NewGuid())); return this->tool->GetLifeCycleState() == LifeCycleState::InActive; }