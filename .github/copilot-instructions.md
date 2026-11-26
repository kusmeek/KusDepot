## Workspace search

- Prefer targeted searches over workspace-wide recursive searches.
- Scope searches to the smallest relevant directory and file pattern, such as `*.cs`, `*.xml`, `*.json`, or `*.md`.
- Use exact filename or path lookup before searching file contents when the likely file name is known.
- Do not search `**/*` across the entire workspace unless there is no narrower alternative.
- Exclude generated and high-volume directories from broad searches, including `.vs`, `bin`, `obj`, `node_modules`, and generated-code folders.
- Use a small result limit first; broaden the scope or limit only when the result is insufficient.
- If a search is canceled or times out, do not immediately repeat the same search. Narrow the path and file pattern, then retry.
- For multiple possible terms, use one scoped regular-expression search rather than several unrestricted searches.
