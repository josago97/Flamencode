# Flamencode

Web editor https://josago97.github.io/Flamencode/

| Flamencode | Brainfuck |  Description |  
|---|---|---|
| asi | > | Increment the data pointer (to point to the next cell to the right) |
| anda | < | Decrement the data pointer (to point to the next cell to the left) |
| ole | + | Increment (increase by one) the byte at the data pointer |
| arsa | - | Decrement (decrease by one) the byte at the data pointer |
| mira | . | Output the byte at the data pointer |
| toma | , | Accept one byte of input, storing its value in the byte at the data pointer |
| dale | [ | If the byte at the data pointer is zero, then instead of moving the instruction pointer forward to the next command, jump it forward to the command after the matching `arre` command |
| arre | ] | If the byte at the data pointer is nonzero, then instead of moving the instruction pointer forward to the next command, jump it back to the command after the matching `dale` command |
