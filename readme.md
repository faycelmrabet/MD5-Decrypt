# Decrypt MD5
C# Application for decrypting MD5 Key using Dictionnary and Anagram.
Find the secret phrase which is an anagram of "poultry outwits ants"


Pre-Analysis
-----------

This challenge was a bit more difficult to solve than I first expected. 
My first thought was to use a classic dictionary attack, but with a glossary of more than 100000 words of which you must select 3 words in the right order, the total number of posible right combination is 100000<sup>3</sup>.

In other words - I had to try a different strategy.
My strategy is, in short, reduce the number of possible words in the list. 
In order to do that, I have the following assumption:

```
> Candidates/words must contain letters included in the anagram and minumum be the same length as the anagram-word
```

This assumption will reduce the list of possible words to (305 x 305 x 539) - still many - but within reach

![Pre-Analysis](https://cloud.githubusercontent.com/assets/20291416/20771120/73890fca-b748-11e6-8de6-b46c90ecd5cb.PNG)


Analysis Step
Next step is to loop through the candidates and generate the MD5 hash for all combinations. 
All combinations are saved to file, in order to emulate a Rainbow table, which I use to lookup the the secret phrase with the MD5 hash:

*MD5 hash*: ***4624d200580677270a54ccff86b9610e***

![Analysis](https://cloud.githubusercontent.com/assets/20291416/20771121/738cfea0-b748-11e6-8941-d4bd564496d4.PNG)


Processing & Result
-----------
This process will run for 1-2 hour and generate af file with posible combinations of 3GB.


The secret phrase with the MD5 hash: 4624d200580677270a54ccff86b9610e is: 
***pastils turnout towy***

![Processing & Result](https://cloud.githubusercontent.com/assets/20291416/20771119/73462d54-b748-11e6-9515-2760eb8f6bb5.PNG)
