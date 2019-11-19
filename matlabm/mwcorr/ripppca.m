      function [p, w, t, l] = ripppca(a, maxrank);

% PCA   Calculate factors and scores by NIPALS algorithm.

%
% Where:
%
%  p        is the matrix of spectral factors
%  w        is the matrix of weights
%  t        is the matrix of spectral intensity
%  x        is the matrix of spectral residuals
%  l        is the vector containing the numbers of iterations required
%  a        is the training set absorbance matrix
%  maxrank  is optional limit to the number of factors tested
%
% Explaination for the subscripts 
%------------------------------------------------
% p     the number of spectrum data points
% j     the number of samples
% i     the factor number
% h     the iteration variable
% k     the increase countor

% Explaination for the dimension of the matrixs or the vectors
%------------------------------------------------
% a                  [p,j]
% told(:,h)          [j,1]
% t(:,h)             [j,1]
% W(h,:)             [1,p]
% p(h,:)             [1,p]
% x                  [j,p]
% t                  [j,i]
% p                  [i,p]
% w                  [j,p]



[p,j] = size(a);
k = 0;
x = a';
if nargin == 2, i = maxrank; end
p=zeros(i,p);
for h = 1:i,
	told(:,h) = zeros(j,1);
	t(:,h) = ones(j,1);
	while (sum(abs(told(:,h)-t(:,h)))) > (1e-15) & k < 100
		told(:,h) = t(:,h);
		k = k + 1;
		w(h,:)  = t(:,h)' * x / (t(:,h)' * t(:,h));
		w(h,:)  =  w(h,:) / sqrt(sum(w(h,:) .* w(h,:)));
		t(:,h)  = x * w(h,:)' / (w(h,:) * w(h,:)');
	end
        l(h) = k;
	k = 0;
p(h,:) = t(:,h)' * x / (t(:,h)' * t(:,h));
p(h,:) = p(h,:) / sqrt(sum(p(h,:) .* p(h,:)));
t(:,h)  = t(:,h) /  sqrt(sum(p(h,:) .* p(h,:)));
w(h,:)  = w(h,:) /  sqrt(sum(p(h,:) .* p(h,:)));
x = x - (t(:,h) * p(h,:));
end
