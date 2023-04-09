# Flamencode
Flamencode is a very low-level joke programming language made with C# and it is based on the Brainfuck language.This language uses some of the typical expressions used in flamenco songs as the name of the operators.

## Syntax
Comments start from the '#' character and end at the line break.
| Flamencode | Brainfuck |  Description |  
|---|---|---|
| anda | > | Increment the data pointer (to point to the next cell to the right) |
| asi | < | Decrement the data pointer (to point to the next cell to the left) |
| ole | + | Increment (increase by one) the byte at the data pointer |
| arsa | - | Decrement (decrease by one) the byte at the data pointer |
| mira | . | Output the byte at the data pointer |
| toma | , | Accept one byte of input, storing its value in the byte at the data pointer |
| dale | [ | If the byte at the data pointer is zero, then instead of moving the instruction pointer forward to the next command, jump it forward to the command after the matching `arre` command |
| arre | ] | If the byte at the data pointer is nonzero, then instead of moving the instruction pointer forward to the next command, jump it back to the command after the matching `dale` command |

## Example Hello World
In the [Examples folder](https://github.com/josago97/Flamencode/tree/main/Examples) there are some sample implementations, example Hello World:
```
# Prints Hello World!

ole ole ole ole ole ole ole ole               # Set Cell 0 to 8
			
dale                                          # Start loop
  anda ole ole ole ole                        # Go to Cell 1 and set 4
  dale                                        # As the cell will be cleared by the loop
    anda ole ole                              # Add 4*2 to Cell 2
    anda ole ole ole                          # Add 4*3 to Cell 3
    anda ole ole ole                          # Add 4*3 to Cell 4
    anda ole                                  # Add 4 to Cell 5
    asi asi asi asi arsa                      # Decrement the loop counter in Cell 1
  arre                                        # Loop until Cell 1 is zero
  anda ole                                    # Add 1 to Cell 2
  anda ole                                    # Add 1 to Cell 3
  anda arsa                                   # Subtract 1 from Cell 4
  anda anda ole                               # Add 1 to Cell 6
  dale asi arre                               # Move back to the first zero cell you find; this will be Cell 1 which was cleared by the previous loop
  asi arsa                                    # Decrement the loop Counter in Cell 0
arre                                          # Loop until Cell 0 is zero

# The result of this is:
# Cell No :   0   1   2   3   4   5   6
# Contents:   0   0  72 104  88  32   8
# Pointer :   ^

anda anda toma                                # Cell 2 has value 72 which is 'H'
anda arsa arsa arsa toma                      # Subtract 3 from Cell 3 to get 101 which is 'e'
ole ole ole ole ole ole ole toma toma         # Likewise for 'll' from Cell 3
ole ole ole toma                              # Likewise for 'o' from Cell 3
anda anda toma                                # Cell 5 is 32 for the space
asi arsa toma                                 # Subtract 1 from Cell 4 for 87 to give a 'W'
asi toma                                      # Cell 3 was set to 'o' from the end of 'Hello'
ole ole ole toma                              # Cell 3 for 'r'
arsa arsa arsa arsa arsa arsa toma            # Cell 3 for 'l'
arsa arsa arsa arsa arsa arsa arsa arsa toma  # Cell 3 for 'd'
anda anda ole toma                            # Add 1 to Cell 5 gives us an exclamation point
anda ole ole toma                             # And finally a newline from Cell 6
```
## How to use
To run a file code, you can download the executable corresponding to your operating system and type in the shell
```
Flamencode.exe /myFolder/myCode.flam
```
Also there is a [web editor](https://josago97.github.io/Flamencode/).
