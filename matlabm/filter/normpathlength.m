% 归一化：用于消除光程变化或样品稀释等变化对光谱产生的影响，常指矢量归一化，
% 首先，计算出光谱的y平均值，再用光谱减去该平均值，这样光谱的中值为零，
% 其次，计算所有的y值的平方和，然后用光谱除以该平方和的平方根，结果光谱的矢量归一化是1。
% 输入x，列向量，校正前光谱；
% 输出xx，列向量，校正后光谱；
function [xx,mx,sd]=normpathlength(x)

	[n,m]=size(x);
	for i=1:m
		mx(i)=mean(x(:,i));
		xb(:,i)=(x(:,i)-mx(i));
		sd(i)=sqrt(xb(:,i)'*xb(:,i));
		xx(:,i)=xb(:,i)./sd(i);
	end
end
