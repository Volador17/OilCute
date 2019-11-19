function w = initc(p,s)
%INITC Inititialize competitive layer.
%	
%	INITC(P,S)
%	  P - Rx2 matrix of input vectors.
%	  S - Number of neurons in layer.
%	Returns SxR weight matrix.
%	
%	IMPORTANT: Each ith row of P must contain expected
%	  min and max values for the ith input.
%	
%	EXAMPLE: W = initc([-2 2;0 5],3)
%	         p = [1 2 3]';
%	         a = simuc(p,W)
%	
%	See also INITFUN, COMPNET, SIMUC, TRAINC.

% Mark Beale, 12-15-93
% Copyright (c) 1992-94 by the MathWorks, Inc.
% $Revision: 1.1 $  $Date: 1994/01/11 16:24:41 $

% INPUTS
[R,Q] = size(p);
if Q < 2,error('First argument did not have multiple columns.'),end

[w,b] = midpoint(s,p);
