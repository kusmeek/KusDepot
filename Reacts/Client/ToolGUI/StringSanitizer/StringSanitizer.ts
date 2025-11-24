export default class StringSanitizer
{
    private static maxLength = 200; private static forbiddenChars = /[<>(){}\[\]\/\\|]/;

    static sanitize(input: string | null | undefined): string | null
    {
        if (!input || input.length > StringSanitizer.maxLength) return null;

        if (StringSanitizer.forbiddenChars.test(input)) return null;

        return input;
    }
}