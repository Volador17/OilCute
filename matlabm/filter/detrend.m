% 去除线性趋势
% input
% x: 光谱
% output
% y: 处理后结果

%DETREND Remove a linear trend from a vector, usually for FFT processing.
%   Y = DETREND(X) removes the best straight-line fit linear trend from 
%   the data in vector X and returns it in vector Y.  If X is a matrix,
%   DETREND removes the trend from each column of the matrix.
%
%   Y = DETREND(X,'constant') removes just the mean value from the vector X,
%   or the mean value from each column, if X is a matrix.
%
%   Y = DETREND(X,'linear',BP) removes a continuous, piecewise linear trend.
%   Breakpoint indices for the linear trend are contained in the vector BP.
%   The default is no breakpoints, such that one single straight line is
%   removed from each column of X.
%
%   See also MEAN

%   Author(s): J.N. Little, 6-08-86
%   	   J.N. Little, 2-29-88, revised
%   	   G. Wolodkin, 2-02-98, added piecewise linear fit of L. Ljung
%   Copyright 1984-2002 The MathWorks, Inc. 
%   $Revision: 1.9 $  $Date: 2002/06/05 17:30:51 $
function y = detrend(x);


	if nargin < 2, o  = 1; end
	if nargin < 3, bp = 0; end

	n = size(x,1);
	if n == 1,
		x = x(:);			% If a row, turn into column vector
	end
	N = size(x,1);

	switch o
		case {0,'c','constant'}
			y = x - ones(N,1)*mean(x);	% Remove just mean from each column

		case {1,'l','linear'}
			bp = unique([0;bp(:);N-1]);	% Include both endpoints
			lb = length(bp)-1;
			a  = [zeros(N,lb) ones(N,1)];	% Build regressor with linear pieces + DC
			for kb = 1:lb
				M = N - bp(kb);
				a([1:M]+bp(kb),kb) = [1:M]'/M;
			end
			y = x - a*(a\x);		% Remove best fit

	otherwise
		% This should eventually become an error.
		warning('MATLAB:detrend:InvalidTrendType', ...
			  'Invalid trend type ''%s''.. assuming ''linear''.',num2str(o));
		y = detrend(x,1,bp); 

	end

	if n == 1
		y = y.';
	end
end
