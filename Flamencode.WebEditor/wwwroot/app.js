﻿export function registerLanguage(language, theme, commentToken, singleTokens, pairTokens) {
	monaco.languages.register({ id: language });

	const allTokens = singleTokens.concat(pairTokens);

	// Register a tokens provider for the language
	monaco.languages.setMonarchTokensProvider(language, {
		tokenizer: {
			root: [
				[new RegExp(`\\b(?:${allTokens.join("|")})\\b`, 'gi'), "token"],
				[new RegExp(`${commentToken}.*`, 'gi'), "comment"],
			],
		},
	});

	// Define a new theme that contains only rules that match this language
	monaco.editor.defineTheme(theme, {
		base: "vs",
		inherit: false,
		rules: [
			{ token: "token", foreground: "0000ff", fontStyle: "bold" },
			{ token: "comment", foreground: "008800", fontStyle: "bold" },
		],
		colors: {
			"editor.foreground": "#000000",
		},
	});

	// Register a completion item provider for the new language
	monaco.languages.registerCompletionItemProvider(language, {
		provideCompletionItems: (model, position) => {
			var word = model.getWordUntilPosition(position);
			var range = {
				startLineNumber: position.lineNumber,
				endLineNumber: position.lineNumber,
				startColumn: word.startColumn,
				endColumn: word.endColumn,
			};

			var singleTokenSuggestions = allTokens.map(t => ({
				label: t,
				kind: monaco.languages.CompletionItemKind.Text,
				insertText: t,
				range: range,
			}));

			var pairTokenSuggestions = [];

			for (let i = 0; i < pairTokens.length; i += 2) {
				let suggestion = {
					label: pairTokens[i] + "-" + pairTokens[i + 1],
					kind: monaco.languages.CompletionItemKind.Snippet,
					insertText: [
						pairTokens[i],
						"\t$0",
						pairTokens[i + 1],
					].join("\n"),
					insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
					range: range
				};

				pairTokenSuggestions.push(suggestion);
			}

			var suggestions = singleTokenSuggestions.concat(pairTokenSuggestions);

			return { suggestions: suggestions };
		},
	});

	// Get current editor and update language and theme
	const editor = monaco.editor.getEditors()[0];
	monaco.editor.setModelLanguage(editor.getModel(), language);
	monaco.editor.setTheme(theme);
}

export function saveFile(content, filename) {
	var link = document.createElement('a');
	link.download = filename;
	link.href = "data:text/plain;charset=utf-8," + encodeURIComponent(content);
	document.body.appendChild(link);
	link.click();
	document.body.removeChild(link);
}




