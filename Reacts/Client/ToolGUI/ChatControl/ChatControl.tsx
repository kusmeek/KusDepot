import React from "react";
import "./ChatControl.css";

export interface ChatItem
{
    id: string;
    sender: string;
    message: string;
}

interface ChatControlState
{
    input: string;
}

interface ChatControlProps
{
    name: string;
    messages: ChatItem[];
    onDismiss: () => void;
    setName: (name: string) => void;
    forwardedRef?: React.RefObject<HTMLDivElement>;
    onSendMessage: (sender: string, message: string) => void;
}

export default class ChatControl extends React.Component<ChatControlProps,ChatControlState>
{
    private inputRef: React.RefObject<HTMLInputElement>;
    private messageListRef: React.RefObject<HTMLDivElement>;

    constructor(props: ChatControlProps)
    {
        super(props);

        this.state = {
            input: ""
        };

        this.inputRef = React.createRef<HTMLInputElement>();
        this.messageListRef = React.createRef<HTMLDivElement>();
    }

    componentDidMount()
    {
        try
        {
            if (this.inputRef.current)
            {
                this.inputRef.current.focus();
            }
            if (this.messageListRef.current)
            {
                requestAnimationFrame(() =>
                {
                    const el = this.messageListRef.current;
                    if (el)
                    {
                        try
                        {
                            el.scrollTo({ top: el.scrollHeight, behavior: 'smooth' });
                        }
                        catch
                        {
                            el.scrollTop = el.scrollHeight;
                        }
                    }
                });
            }
        }
        catch (error)
        {
            console.error('[ChatControl] Error in componentDidMount:',error);
        }
    }

    componentDidUpdate(prevProps: ChatControlProps)
    {
        try
        {
            const messagesChanged = prevProps.messages.length !== this.props.messages.length;

            if (messagesChanged && this.messageListRef.current)
            {
                const el = this.messageListRef.current;
                try
                {
                    el.scrollTo({ top: el.scrollHeight, behavior: 'smooth' });
                }
                catch
                {
                    el.scrollTop = el.scrollHeight;
                }
            }
        }
        catch (error)
        {
            console.error('[ChatControl] Error in componentDidUpdate:',error);
        }
    }

    private handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) =>
    {
        try
        {
            this.setState({ input: e.target.value });
        }
        catch (error)
        {
            console.error('[ChatControl] Error in handleInputChange:',error);
        }
    };

    private handleSend = (e: React.FormEvent | React.MouseEvent) =>
    {
        try
        {
            e.preventDefault();
            const { name } = this.props;
            const { input } = this.state;
            if (!name || !input) return;
            this.props.onSendMessage(name, input);
            this.setState({ input: "" });
        }
        catch (error)
        {
            console.error('[ChatControl] Error in handleSend:',error);
        }
    };

    private handleSetName = (e: React.FormEvent | React.MouseEvent) =>
    {
        try
        {
            e.preventDefault();
            if (!this.state.input) return;
            this.props.setName(this.state.input);
            this.setState({ input: "" });
        }
        catch (error)
        {
            console.error('[ChatControl] Error in handleSetName:',error);
        }
    };

    private handleInputKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) =>
    {
        try
        {
            e.stopPropagation();
            if (e.key === "Escape" || e.key === '`')
            {
                this.props.onDismiss();

                return;
            }
            if (e.key === "Enter")
            {
                if (this.props.name)
                {
                    this.handleSend(e);
                }
                else
                {
                    this.handleSetName(e);
                }
            }
        }
        catch (error)
        {
            console.error('[ChatControl] Error in handleInputKeyDown:',error);
        }
    };

    private handleMessageListKeyDown = (e: React.KeyboardEvent<HTMLDivElement>) =>
    {
        try
        {
            if (e.key === '`')
            {
                this.props.onDismiss();
            }
        }
        catch (error)
        {
            console.error('[ChatControl] Error in handleMessageListKeyDown:',error);
        }
    };

    render()
    {
        try
        {
            const { messages, name, forwardedRef } = this.props;
            const { input } = this.state;
            return (
                <div
                    className="overlay"
                    ref={forwardedRef}
                    onClick={e => e.stopPropagation()}
                >
                    <div
                        ref={this.messageListRef}
                        className="message-list hide-scrollbar"
                        onClick={e => e.stopPropagation()}
                        onKeyDown={this.handleMessageListKeyDown}
                    >
                        {messages.map((msg) => (
                            <div key={msg.id} className="message">
                                <span className="sender">{msg.sender}</span> {msg.message}
                            </div>
                        ))}
                    </div>
                    <form className="input-row" onSubmit={name ? this.handleSend : this.handleSetName}>
                        <input
                            type="text"
                            className="input"
                            value={input}
                            onChange={this.handleInputChange}
                            onKeyDown={this.handleInputKeyDown}
                            placeholder={name ? "say..." : "name..."}
                            autoComplete="off"
                            ref={this.inputRef}
                        />
                    </form>
                </div>
            );
        }
        catch (error)
        {
            console.error('[ChatControl] Error in render:',error);
            return null;
        }
    }
}