function [w1,b1,w2,b2,i,tr]=tbp2(w1,b1,f1,w2,b2,f2,p,t,tp)
	        
%	[W1,B1,W2,B2,TE,TR] = TBP2(W1,B1,F1,W2,B2,F2,P,T,TP)
%	  Wi - SixR weight matrix of ith layer.
%	  Bi - Six1 bias vector of ith layer.
%	  F  - Transfer function (string) of ith layer.
%	  P  - RxQ matrix of input vectors.
%	  T  - S2xQ matrix of target vectors.
%	  TP - Training parameters (optional).
%	Returns:
%	  Wi - new weights.
%	  Bi - new biases.
%	  TE - the actual number of epochs trained.
%	  TR - training record: [row of errors]
%	
%	Training parameters are:
%	  TP(1) - Epochs between updating display, default = 25.
%	  TP(2) - Maximum number of epochs to train, default = 1000.
%	  TP(3) - Sum-squared error goal, default = 0.02.
%	  TP(4) - Learning rate, 0.01.
%	Missing parameters and NaN's are replaced with defaults.

if nargin < 8,error('Not enough arguments.'),end

% TRAINING PARAMETERS
if nargin == 7, tp = []; end
tp = nndef(tp,[25 1000 0.02 0.01]);
df = tp(1);
me = tp(2);
eg = tp(3);
lr = tp(4);
df1 = feval(f1,'delta');
df2 = feval(f2,'delta');

% PRESENTATION PHASE
a1 = feval(f1,w1*p,b1);
a2 = feval(f2,w2*a1,b2);
e = t-a2;
SSE = sumsqr(e);



for i=1:me

  % CHECK PHASE
  if SSE < eg, i=i-1; break, end

  % BACKPROPAGATION PHASE
  d2 = feval(df2,a2,e);
  d1 = feval(df1,a1,d2,w2);

  % LEARNING PHASE
  [dw1,db1] = learnbp(p,d1,lr);
  [dw2,db2] = learnbp(a1,d2,lr);
  w1 = w1 + dw1; b1 = b1 + db1;
  w2 = w2 + dw2; b2 = b2 + db2;

  % PRESENTATION PHASE
  a1 = feval(f1,w1*p,b1);
  a2 = feval(f2,w2*a1,b2);
  e = t-a2;
  SSE = sumsqr(e);
 
end

